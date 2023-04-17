using ICSSoft.STORMNET.Business;
using ITSK.AzimuthPT.JsonFlowReader.Service.Object;
using ITSK.AzimuthPT.JsonFlowReader.Service.ObjectsToSettings;
using Newtonsoft.Json;
using ITSK.AzimuthPT.GetJsonMsgFlow.Lib;
using ITSK.AzimuthPT.GetJsonMsgFlow.Lib.Objects;
using ITSK.PMPTP;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.FunctionalLanguage.SQLWhere;
using Connection = ITSK.AzimuthPT.JsonFlowReader.Service.Object.Connection;
using Root = ITSK.AzimuthPT.GetJsonMsgFlow.Lib.Objects.Root;
using ITSK.AzimuthPT.JsonFlowReader.Service.logTest;
using System.Diagnostics.Metrics;
using ITSK.COP.Utils.Extensions;
using static ITSK.COP.Utils.ORM.DataServiceFactory;
using COUB.Utils.Extensions;
using COUB.Utils.Messaging;
using COUB.WeblessUtils;
using ITSK.COP.Utils.DI;
using ITSK.COP.Utils.Logging;
using ITSK.COP.Utils.ORM;
using ITSK.COUB;
using Chilkat;

namespace ITSK.AzimuthPT.JsonFlowReader.Service
{
    public class Worker : BackgroundService
    {
        public static string con_id { get; set; }

        private readonly ILogger _logger;

        public IConfiguration Configuration { get; set; }

        private static IDataService _dataService;
        public Worker(IConfiguration config, IDataService dataService)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                //builder.Services.AddConfigure(config);
                builder.AddConfiguration(config);
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error);
                builder.AddConsole();
            });
            loggerFactory.AddFile(Path.Combine("C:\\logs\\", "ITSK.AzimuthPT.JsonFlowReader.Service.Log.txt"));

            _logger = loggerFactory.CreateLogger("ITSK.TestLogger");
            _dataService = dataService;
            _logger.LogInformation($@"Trying to build configuration.");
            Configuration = config;
            _logger.LogInformation($@"Build configuration successful.");

        }
        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($@"Service run.");
            try
            {
                //while (true)
                //{
                //    FFmessage ffmessage = new FFmessage();

                //    ffmessage.carBrand = "1";
                //    ffmessage.carClass = "1";
                //    ffmessage.carModel = "1";

                //    DataObject dobj = (FFmessage)ffmessage;
                //    SendMessage<DataObject>(dobj);
                //    TimeSpan.FromMilliseconds(3000000);
                //}
                _logger.LogInformation($@"Trying to read config.");
                Root rootSettings = Configuration.Get<Root>();
                _logger.LogInformation($@"Read config successful.");
                try
                {
                    GetJsonMsg jsonMsg = null;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            if (jsonMsg == null)
                            {
                                jsonMsg = new GetJsonMsg();
                                jsonMsg._loggerFlow = _logger;
                                jsonMsg.JsonReceivedEvent += JsonMsg_JsonReceivedEvent;

                                jsonMsg.Run();
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                        }
                    }

                    if (jsonMsg != null)
                        jsonMsg.Stop();

                    _logger.LogInformation("Program is stopped");
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            _logger.LogInformation($@"Service stop.");
        }

        private void JsonMsg_JsonReceivedEvent(object sender, JsonEventArgs e)
        {
            IDataService ds = _dataService;
            Connection myDeserializedClass = new Connection();
            ConnectionFunc func = new ConnectionFunc();
            string msg = func.GetStr(e.Json);
            myDeserializedClass = JsonConvert.DeserializeObject<Connection>(msg);
            if (myDeserializedClass.connection_id != null)
                con_id = myDeserializedClass.connection_id;
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() =>
            {
                bool sucs = false;
                while (!sucs)
                {
                    IJsonParser parser = new DesToMsg(ds);
                    parser.SetLog(_logger);
                    DataObject[] reff = null;
                    try
                    {
                        reff = parser.GetDesJson(e.Json, con_id);
                        foreach (DataObject dataObj in reff)
                        {
                            SendMessage<DataObject>(dataObj);
                        }
                        //_dataService.UpdateObjects(ref reff);
                        sucs = true;
                        _logger.LogInformation($@"Successful update DB.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
            });
        }
        public void SendMessage<T>(T message, DateTime startProcessAt = default(DateTime)) where T : ICSSoft.STORMNET.DataObject
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageName = message.GetType().Name;
            try
            {
                var apiResult = COUBMessaging.Register(messageName, message,
                    startProcessAt: startProcessAt == default(DateTime) ? DateTime.Now : startProcessAt);
                if (apiResult != CoubMessageApiResult.SuccessfullyRegistered)
                    _logger.LogError(() => $"The result of sending the message {messageName}: {apiResult}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, () => $"Error when sending a message {messageName}");
            }
        }

    }
}
