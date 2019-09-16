using System;
using System.Timers;
using System.Collections.Generic;
using System.Media;

namespace Nochnik
{
    class AlarmClock
    {
        readonly List<Timer> alarms = new List<Timer>();
        readonly SoundPlayer workPlayer = new SoundPlayer(Properties.Resources.work);
        readonly SoundPlayer understoodPlayer = new SoundPlayer(Properties.Resources.understood);

        public void AddAlarm(int targetMinutes)
        {
            int currentMinutes = DateTime.Now.Minute;
            int currentSeconds = DateTime.Now.Second;
            int difference = targetMinutes - currentMinutes;
            if (difference < 0) difference += 60;

            Timer alarm = new Timer(difference * 60 * 1000 - currentSeconds * 1000);
            alarm.AutoReset = false;
            alarm.Elapsed += (sender, e) =>
            {
                workPlayer.Play();
                alarm.Dispose();
            };
            alarm.Start();
            alarms.Add(alarm);
            understoodPlayer.Play();
        }

        public void RemoveAlarms()
        {
            foreach (Timer alarmClock in alarms)
            {
                alarmClock.Stop();
                alarmClock.Dispose();
            }
            alarms.Clear();
            understoodPlayer.Play();
        }
    }
}
