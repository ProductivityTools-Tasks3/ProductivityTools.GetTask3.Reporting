using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductivityTools.GetTask3.Reporting
{
    internal class ReportMd
    {
        private static Dictionary<Contract.ElementView, Contract.ElementView> CaseParent = new Dictionary<Contract.ElementView, Contract.ElementView>();
        private static List<Contract.ElementView> Closed = new List<Contract.ElementView>();

        public static string PrepareReport(Contract.ElementView rootElement)
        {
            RemoveNotFinishedElements(rootElement);
            StringBuilder sb = new StringBuilder();
            BuildMdBFS(rootElement, sb, 0);
            var r = sb.ToString();
            return r;
            //FindClosedMd(rootElement);
            //BuildMD();
        }

        private static string GetMarkdown(int amount)
        {
            string result = "";
            if (amount < 1)
            {
                result += GetHash(amount);
            }
            else
            {
                result += GetHypen(amount);
            }
            result += " ";
            return result;
        }
        private static string GetHash(int amount)
        {
            string result = "";
            for (int i = 0; i <= amount; i++)
            {
                result += "#";

            }
            return result;
        }

        private static string GetHypen(int amount)
        {
            string result = "";
            for (int i = 1; i < amount; i++)
            {
                result += "  ";
            }
            result += "-";
            return result;
        }




        private static void BuildMdBFS(Contract.ElementView element, StringBuilder sb, int level)
        {
            sb.Append(GetMarkdown(level) + element.Name + Environment.NewLine);
            foreach (var item in element.Elements)
            {
                BuildMdBFS(item, sb, level + 1);
            }

        }
        //private static void FindClosedMd(Contract.ElementView element)
        //{
        //    if (element.Finished.HasValue && element.Finished.Value > DateTime.Now.AddDays(-1))
        //    {
        //        Closed.Add(element);
        //    }
        //    foreach (var item in element.Elements)
        //    {
        //        CaseParent.Add(item, element);
        //        FindClosedMd(item);
        //    }
        //}

        private static DateTime? startOfTheWeek;
        private static DateTime StartOfTheWeek
        {
            get
            {
                if(startOfTheWeek == null)
                {
                    DateTime now = DateTime.Now;
                    int dayOfWeek = (int)now.DayOfWeek-1;

                    if (dayOfWeek < 0)
                    {// day of week is Sunday and we want to use Monday as the start of the week
                     // Sunday is now the seventh day of the week
                        dayOfWeek = 6;
                    }

                    startOfTheWeek= now.AddDays(-1 * (double)dayOfWeek).Date;
                }
                return startOfTheWeek.Value;
            }
            
        }



        private static bool RemoveNotFinishedElements(Contract.ElementView element)
        {
            for (int i = element.Elements.Count - 1; i >= 0; i--)
            {
                var result = RemoveNotFinishedElements(element.Elements[i]);
                if (result == false)
                {
                    element.Elements.Remove(element.Elements[i]);
                }
            }

            if ((element.Finished.HasValue && element.Finished.Value > StartOfTheWeek) || element.Elements.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //public static string BuildMD()
        //{
        //    string result = string.Empty;
        //    foreach (var item in Closed)
        //    {
        //        List<string> list = new List<string>();
        //        list.Add(item.Name);
        //        var element = item;
        //        while (CaseParent.ContainsKey(CaseParent[element]))
        //        {
        //            element = CaseParent[element];
        //            list.Add(element.Name);
        //        }

        //        result += FormatMD(list);
        //    }

        //    return result;
        //}
        //private static string FormatMD(List<string> input)
        //{
        //    string result = string.Empty;
        //    input.Reverse();
        //    for (int i = 0; i < input.Count; i++)
        //    {
        //        if (i == 0)
        //        {
        //            result += string.Format($"# {input[0]}") + Environment.NewLine;
        //        }
        //        if (i == 1)
        //        {
        //            result += string.Format($"## {input[1]}") + Environment.NewLine;
        //        }
        //        if (i == 2)
        //        {
        //            result += string.Format($"- {input[2]}") + Environment.NewLine;
        //        }
        //        if (i == 3)
        //        {
        //            result += string.Format($"  - {input[3]}") + Environment.NewLine;
        //        }
        //        if (i > 4)
        //        {
        //            result += string.Format($"  - $ {input[3]}") + Environment.NewLine;

        //        }
        //    }
        //    return result;

        //}
    }
}
