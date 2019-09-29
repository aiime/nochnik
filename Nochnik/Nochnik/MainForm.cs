using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using System.Xml;

namespace Nochnik
{
    partial class MainForm : Form
    {
        public WallpaperClock wallpaperClock;
        WallpaperPainter wallpaperPainter;
        UserController userController;

        public MainForm()
        {
            wallpaperPainter = new WallpaperPainter();
            wallpaperClock = new WallpaperClock(wallpaperPainter);
            userController = new UserController(wallpaperPainter);

            InitializeComponent();
            CreateProgramTray();
            wallpaperClock.Start(1000);
            StartNochnikEveryDay();
            StopNochnikEveryDay();
            KeyDetector.Start(this);
        }

        void CreateProgramTray()
        {
            trayIcon.ContextMenu = new ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Users", (s, a) => { new UsersForm(userController).Show(); }));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Position", new EventHandler(TrayIcon_ClockProperties)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Color", new EventHandler(TrayIcon_Color)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("-"));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(TrayIcon_Exit)));
            trayIcon.Visible = true;
        }

        void TrayIcon_Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void TrayIcon_ClockProperties(object sender, EventArgs e)
        {
            new ClockPropertiesForm(this).Show();
        }

        void TrayIcon_Color(object sender, EventArgs e)
        {
            new ColorForm(wallpaperClock).Show();
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            wallpaperPainter.SetInitialWallpaper();
        }

        void StartNochnikEveryDay()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<StartNochnikJob>()
                .WithIdentity("StartNochnik")
                .Build();

            job.JobDataMap["wallpaperClock"] = wallpaperClock;
            job.JobDataMap["userController"] = userController;

            XmlDocument timeInfo = new XmlDocument();
            timeInfo.Load(AppDomain.CurrentDomain.BaseDirectory + @"\time_info.xml");
            int shiftStartHour = Int32.Parse(timeInfo.GetElementsByTagName("shift_start_hour")[0].InnerText);
            int shiftStartMinute = Int32.Parse(timeInfo.GetElementsByTagName("shift_start_minute")[0].InnerText);
            int shiftStartSecond = Int32.Parse(timeInfo.GetElementsByTagName("shift_start_second")[0].InnerText);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("StartNochnikTrigger")
                .StartAt(DateBuilder.TodayAt(shiftStartHour, shiftStartMinute, shiftStartSecond))
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        void StopNochnikEveryDay()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<StopNochnikJob>()
                .WithIdentity("StopNochnik")
                .Build();

            job.JobDataMap["wallpaperClock"] = wallpaperClock;
            job.JobDataMap["userController"] = userController;

            XmlDocument timeInfo = new XmlDocument();
            timeInfo.Load(AppDomain.CurrentDomain.BaseDirectory + @"\time_info.xml");
            int shiftEndHour = Int32.Parse(timeInfo.GetElementsByTagName("shift_end_hour")[0].InnerText);
            int shiftEndMinute = Int32.Parse(timeInfo.GetElementsByTagName("shift_end_minute")[0].InnerText);
            int shiftEndSecond = Int32.Parse(timeInfo.GetElementsByTagName("shift_end_second")[0].InnerText);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("StopNochnikTrigger")
                .StartAt(DateBuilder.TodayAt(shiftEndHour, shiftEndMinute, shiftEndSecond))
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        class StartNochnikJob : IJob
        {
            async Task IJob.Execute(IJobExecutionContext context)
            {
                (context.JobDetail.JobDataMap["wallpaperClock"] as WallpaperClock).Start(1000);
                (context.JobDetail.JobDataMap["userController"] as UserController).SetUserStatusesAccordingToSchedule();
                (context.JobDetail.JobDataMap["userController"] as UserController).UpdateUserStatuses();
                await Task.CompletedTask;
            }
        }

        class StopNochnikJob : IJob
        {
            async Task IJob.Execute(IJobExecutionContext context)
            {
                (context.JobDetail.JobDataMap["wallpaperClock"] as WallpaperClock).Stop();

                UserController userController = (context.JobDetail.JobDataMap["userController"] as UserController);
                List<User> users = userController.users;
                foreach (User user in users)
                {
                    user.CurrentStatus = UserStatus.AtHome;
                }
                userController.UpdateUserStatuses();

                await Task.CompletedTask;
            }
        }
    }
}
