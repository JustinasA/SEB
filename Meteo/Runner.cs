using System;
using System.Diagnostics;
using System.Reflection;
using Meto;
using NUnitLite;

namespace Meteo
{
    /*----------------------------------------------------------------------------------------------------*/
    /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
    /// <summary>
    /// Executes nunit tests and opens created report
    /// </summary>
    /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
    class Runner
    {
        public static void Main(String[] args)
        {            
            string fitler = "/test:" + typeof(Runner).Namespace;
            new AutoRun(Assembly.GetExecutingAssembly())
                           .Execute(new String[] { fitler });

            Process.Start(TestBase.ReportPath);
        }

    }
}
