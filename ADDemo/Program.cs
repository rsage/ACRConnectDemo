using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ADDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var userName = [provide valid AD user name here];
            var pass = [provide password here];

            var entry = new DirectoryEntry("LDAP://192.168.1.123/DC=restonuat,DC=local", userName, pass);
            var searcher = new DirectorySearcher(entry);
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("mail");
            searcher.Filter = "(&(anr=srusinov)(objectCategory=person))";

            var res = searcher.FindOne();

            foreach (string name in res.Properties.PropertyNames)
            {
                Console.WriteLine("{0}={1}", name, res.Properties[name][0]);   
            }
        }
    }
}
