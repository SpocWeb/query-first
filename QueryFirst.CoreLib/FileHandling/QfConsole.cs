﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst
{
    public class QfConsole
    {
        public static IQfConsole NotTheSystemConsole { get; set; }
        public static void WriteLine(string line)
        {
            if (NotTheSystemConsole != null)
            {
                NotTheSystemConsole.WriteLine(line);
            }
            else Console.WriteLine(line);
        }
    }
    public interface IQfConsole
    {
        void WriteLine(string line);
    }
}
