using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace sandhiya
{
    public class Contract1 : SmartContract
    {
       public static byte[] Authorized;

        public static Object Main(string operation,Object[] args)
        {
            if(operation =="Deploy")
            {
                byte[] aprover = (byte[])args[0];
                return Deploy(aprover);
            }
            if(operation =="AddAuthorizedUser")
            {
                byte[] owner = (byte[])args[0];
                byte[] user = (byte[])args[1];
                return AddAuthorizedUser(owner, user);
            }
            if(operation =="RemoveAuthorizedUser")
            {
                byte[] owner = (byte[])args[0];
                byte[] user = (byte[])args[1];
                return RemoveAuthorizedUser(owner, user);

            }
            if(operation =="ChangeApprover")
            {
                byte[] owner = (byte[])args[0];
                byte[] user = (byte[])args[1];
                return ChangeAprover(owner,user);
            }

            return true;
        }
       public static bool Deploy(byte[] aprover)
        {
            Runtime.Notify("deploy");
            Storage.Put(Storage.CurrentContext, "Aprover", aprover);
            return true;
        }
        public static bool AddAuthorizedUser(byte[] owner,byte[] user)
        {
            Runtime.Notify("AddUsers");
            byte[] admin = Storage.Get(Storage.CurrentContext, "Aprover");
            if(admin!=owner)
            {
                return false;
            }
           byte[] Authorized = user.Concat("abc".AsByteArray());
            Storage.Put(Storage.CurrentContext, user, "true");
            return true;
             
        }
        public static bool RemoveAuthorizedUser(byte[] owner,byte[]  user)
        {
            Runtime.Notify("RemoveUsers");
            byte[] admin = Storage.Get(Storage.CurrentContext, "Aprover");
            if (admin != owner)
            {
                return false;
            }
            byte[] Authorized = user.Concat("abc".AsByteArray());
            Storage.Put(Storage.CurrentContext, user, "false");
            return true;
        }
        public static bool ChangeAprover(byte[] owner,byte[] newuser)
        {
            Runtime.Notify("ChangeApprover");
            byte[] admin = Storage.Get(Storage.CurrentContext, "Aprover");
            if (admin != owner)
            {
                return false;
            }

            Storage.Put(Storage.CurrentContext, "Aprover", newuser);
            return true;


        }
    }
}
