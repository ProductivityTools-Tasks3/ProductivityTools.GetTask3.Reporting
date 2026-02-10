using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductivityTools.GetTask3.Reporting
{
    internal class ReportSimple
    {
        public static string PrepareReport(Contract.ElementView rootElement)
        {
            var result = FindClosed(rootElement.Name, rootElement);
            return result;
        }

        private static string FindClosed(string path, Contract.ElementView element)
        {
            if (element.Finished.HasValue && element.Finished.Value > DateTime.Now.AddDays(-1))
            {
                return string.Concat(path, "\\", element.Name) + Environment.NewLine;
            }
            var r = string.Empty;
            foreach (var item in element.Elements)
            {
                r += FindClosed(element.Name, item);
            }
            return r;
        }
    }
}
