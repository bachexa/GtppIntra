using System;
using System.Threading;
using System.Threading.Tasks;
using Envirovement.Clases;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class ContinuousProcessingService : BackgroundService
{
    private readonly ILogger<ContinuousProcessingService> _logger;
    private readonly Helper _helper;

    public ContinuousProcessingService(ILogger<ContinuousProcessingService> logger, Helper helper)
    {
        _logger = logger;
        _helper = helper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factoryTask = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Calling GetInformationFromFactory.");
                    _helper.GetInformationFromFactory(null);
                    _logger.LogInformation("Completed GetInformationFromFactory.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("GetInformationFromFactory task was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in GetInformationFromFactory task.");
                }
            }
        }, stoppingToken);

        var insertDataTask = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Calling InsertDataexternalDatabaseAsyncCallback.");
                    await Task.Run(() => _helper.InsertDataexternalDatabaseAsyncCallback(null), stoppingToken);
                    _logger.LogInformation("Completed InsertDataexternalDatabaseAsyncCallback.");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("InsertDataexternalDatabaseAsyncCallback task was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in InsertDataexternalDatabaseAsyncCallback task.");
                }
            }
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting a new iteration of continuous processing.");

                // Run the CheckReportAndSendMail task
                _logger.LogInformation("Calling CheckReportAndSendMail.");
                await Task.Run(() => _helper.CheckReportAndSendMail(null), stoppingToken);
                _logger.LogInformation("Completed CheckReportAndSendMail.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                // Run the DeleOldRecords task
                _logger.LogInformation("Calling DeleOldRecords.");
                await Task.Run(() => _helper.DeleOldRecords(null), stoppingToken);
                _logger.LogInformation("Completed DeleOldRecords.");
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

                // Run the CheckLanConnection task
                _logger.LogInformation("Calling CheckLanConnection.");
                await Task.Run(() => _helper.CheckLanConnection(null), stoppingToken);
                _logger.LogInformation("Completed CheckLanConnection.");
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

                // Run the CheckLanAndInsertDatabase task
                _logger.LogInformation("Calling CheckLanAndInsertDatabase.");
                await Task.Run(() => _helper.CheckLanAndInsertDatabase(null), stoppingToken);
                _logger.LogInformation("Completed CheckLanAndInsertDatabase.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                _logger.LogInformation("Completed an iteration of continuous processing.");
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task was canceled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during continuous processing.");
            }
        }

        _logger.LogInformation("Continuous processing service is stopping.");

        // Ensure all tasks are completed before exiting
        await Task.WhenAll(factoryTask, insertDataTask);
    }
}















//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Envirovement.Clases;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

//internal class ContinuousProcessingService : BackgroundService
//{
//    private readonly ILogger<ContinuousProcessingService> _logger;
//    private readonly Helper _helper;

//    public ContinuousProcessingService(ILogger<ContinuousProcessingService> logger, Helper helper)
//    {
//        _logger = logger;
//        _helper = helper;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        //Start the timers here
//        Timer timer5 = new Timer(new TimerCallback(_helper.GetInformationFromFactory), null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
//        Timer timer = new Timer(new TimerCallback(_helper.CheckReportAndSendMail), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
//        //Timer timer2 = new Timer(new TimerCallback(_helper.ConnectionStatusCallback), null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
//        Timer timer3 = new Timer(new TimerCallback(_helper.InsertDataexternalDatabaseAsyncCallback), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
//        Timer timer6 = new Timer(new TimerCallback(_helper.DeleOldRecords), null, TimeSpan.Zero, TimeSpan.FromDays(1));
//        Timer timer4 = new Timer(new TimerCallback(_helper.CheckLanConnection), null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
//        Timer timerCheckLan = new Timer(new TimerCallback(_helper.CheckLanAndInsertDatabase), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                // Put your continuous processing logic here
//                _logger.LogInformation("Continuous processing task is running.");

//                // Delay for a specific duration before running the next iteration
//                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Change the TimeSpan as needed
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred during continuous processing.");
//            }
//        }

//        // Dispose the timers when the service is stopped
//        timer5.Dispose();
//        timer.Dispose();
//        //timer2.Dispose();
//        timer3.Dispose();
//        timer6.Dispose();
//        timer4.Dispose();
//        timerCheckLan.Dispose();
//    }
//}