using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleConfigs.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> WaitAsync<T>(this Task<T> task, int milliseconds)
        {
            return task.WaitAsync(TimeSpan.FromMilliseconds(milliseconds));
        }

        public static Task WaitAsync(this Task task, int milliseconds)
        {
            return task.WaitAsync(TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}
