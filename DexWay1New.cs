using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Testing
{
    public class Contract1 : SmartContract
    {


        public static Object Main(string operation, params object[] args)

        {
            byte[] Approver;
            BigInteger beginTime;
            BigInteger endTime;
            if (operation == "Deploy")
            {
                Approver = (byte[])args[0];
                Storage.Put(Storage.CurrentContext, "approver", Approver);
                Storage.Put(Storage.CurrentContext, "vaultSealed", 0);
                Storage.Put(Storage.CurrentContext, "openVault", 0);
                Storage.Put(Storage.CurrentContext, "contractActive", 1);
                return true;
            }
            if (operation == "Deposit")
            {
                byte[] Account1 = (byte[])args[0];
                BigInteger Amount = (BigInteger)args[1];
                BigInteger AccAmount = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, Account1));
                BigInteger Balance = AccAmount + Amount;
                Runtime.Notify(Balance + 0);
                Storage.Put(Storage.CurrentContext, Account1, Balance);
            }
            if (operation == "balanceof")
            {
                byte[] Address1 = (byte[])args[0];
                BigInteger Value = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, Address1));
                return Value;
            }
            // TradeFeeCalculator
            if (operation == "updateFeeSchedule")
            {
                uint baseTokenFee = (uint)args[0];
                uint etherTokenFee = (uint)args[1];
                uint normalTokenFee = (uint)args[2];
                if (baseTokenFee >= 0 && baseTokenFee <= 1 && etherTokenFee >= 0 && etherTokenFee <= 1 && normalTokenFee >= 0 && normalTokenFee <= 1)
                {
                    Storage.Put(Storage.CurrentContext, "1", baseTokenFee);
                    Storage.Put(Storage.CurrentContext, "2", etherTokenFee);
                    Storage.Put(Storage.CurrentContext, "3", normalTokenFee);
                    return true;
                }


            }
            if (operation == "calcTradeFee")
            {
                BigInteger _value = (BigInteger)args[0];
                BigInteger _feeIndex = (BigInteger)args[1];
                byte[] Index;
                if (_feeIndex >= 0 && _feeIndex <= 2 && _value > 0)
                {
                    if (_feeIndex == 0)
                    {
                        Index = Storage.Get(Storage.CurrentContext, "0");
                    }
                    else if (_feeIndex == 1)
                    {
                        Index = Storage.Get(Storage.CurrentContext, "1");
                    }
                    Index = Storage.Get(Storage.CurrentContext, "2");
                    BigInteger Index1 = new BigInteger(Index);
                    BigInteger _totalFees = _value * (Index1) / 1;
                    if (_totalFees > 0)
                    {
                        return _totalFees;
                    }
                    return false;
                }

            }

            if (operation == "calcTradeFeeMulti")
            {
                BigInteger[] _values = (BigInteger[])args[0];
                BigInteger[] _feeIndexes = (BigInteger[])args[1];
                byte[] index;
                if (_values.Length > 0 && _feeIndexes.Length > 0 && _values.Length == _feeIndexes.Length)
                {
                    BigInteger[] m_totalFees = new BigInteger[_values.Length];

                    for (uint i = 0; i < _values.Length; i++)
                    {
                        if (_feeIndexes[i] >= 0 && _feeIndexes[i] >= 2 && _values[i] > 0)
                        {
                            if (_feeIndexes[i] == 0)
                            {
                                index = Storage.Get(Storage.CurrentContext, "0");

                            }
                            else if (_feeIndexes[i] == 1)
                            {
                                index = Storage.Get(Storage.CurrentContext, "1");

                            }
                            index = Storage.Get(Storage.CurrentContext, "2");
                            BigInteger Indexing = new BigInteger(index);
                            m_totalFees[i] = (_values[i] * Indexing);
                            return m_totalFees;
                        }
                    }
                }
                return false;
            }

            if (operation == "changeApprover")
            {
                byte[] newApprover = (byte[])args[0];
                byte[] Approve = Storage.Get(Storage.CurrentContext, "approver");
                if (Approve != newApprover)
                {
                    Runtime.Notify("check");
                    Approver = newApprover;
                    Runtime.Notify("checking");
                    Storage.Put(Storage.CurrentContext, "approver", Approver);
                    return true;
                }
                return false;
            }
            //Integrator

            if (operation == "addAuthorizedAddress")
            {
                Runtime.Notify("success");
                byte[] appIntegrator = (byte[])args[0];
                // if (Runtime.CheckWitness(Storage.Get(Storage.CurrentContext, "Approver"))) ;
                byte[] Approved = Storage.Get(Storage.CurrentContext, "approver");
                if (Approved != appIntegrator)
                {
                    Storage.Put(Storage.CurrentContext, appIntegrator, 1);
                    return true;
                }
            }
            if (operation == "removeAuthorizedAddress")
            {
                byte[] appIntegrator = (byte[])args[0];
                byte[] Approved = Storage.Get(Storage.CurrentContext, "approver");
                if (Approved != appIntegrator)
                {
                    Storage.Put(Storage.CurrentContext, appIntegrator, 0);
                    // BigInteger ss = new BigInteger(Storage.Get(Storage.CurrentContext, appIntegrator)[0]);
                    if ((Storage.Get(Storage.CurrentContext, appIntegrator)[0]) == 0)
                    {
                        Storage.Delete(Storage.CurrentContext, appIntegrator);
                        Runtime.Notify("removed successfully");
                    }
                }
                return false;
            }
            //EcVerify

            if (operation == "ecrecovery")
            {
                byte[] msg_Hash = (byte[])args[0];
                uint v = (uint)args[1];
                byte[] r = (byte[])args[2];
                byte[] s = (byte[])args[3];
                byte[] address00 = Storage.Get(Storage.CurrentContext, "approver");
                return address00;

            }
            if (operation == "ecVerify")
            {
                byte[] msg_Hash = (byte[])args[0];
                uint v = (uint)args[1];
                byte[] r = (byte[])args[2];
                byte[] s = (byte[])args[3];
                byte[] signer = (byte[])args[4];
                byte[] address00 = Storage.Get(Storage.CurrentContext, "approver");
                if (address00 == signer)
                {
                    return false;
                }
                if (v < 27)
                {
                    v += 27;
                }
                if (v != 27 && v != 28)
                {
                    return address00 == signer;
                }

                if (v == 27)
                {
                    Runtime.Notify("verified");
                    return Main("ecrecover", msg_Hash, v, r, s) == signer;
                }
                else if (v == 28)
                {
                    Runtime.Notify(" Verified");
                    return Main("ecrecover", msg_Hash, v, r, s) == signer;
                }

                return address00 == signer;
            }
            //orderVault
            if (operation == "orderVault")
            {
                byte[] open = Storage.Get(Storage.CurrentContext, "openVault");
                byte[] seal = Storage.Get(Storage.CurrentContext, "vaultSealed");

                if (open.Equals(0) && (seal.Equals(0)))
                {

                    Approver = (byte[])args[0];

                    Storage.Put(Storage.CurrentContext, "Approver", Approver);
                    Runtime.Log("Approver Success");
                    return true;
                }
                return false;
            }
            if (operation == "addOwner")
            {
                byte[] newOwner = (byte[])args[0];
                Storage.Put(Storage.CurrentContext, newOwner, 1);
                return true;
            }

            if (operation == "removeOwner")
            {
                byte[] owner = (byte[])args[0];
                Storage.Put(Storage.CurrentContext, owner, 0);
                //  BigInteger ss1 = new BigInteger(Storage.Get(Storage.CurrentContext, owner)[0]);
                if (Storage.Get(Storage.CurrentContext, owner)[0] == 0)
                {
                    Storage.Delete(Storage.CurrentContext, owner);
                }
                return false;


            }
            if (operation == "openVault")
            {
                beginTime = (BigInteger)args[0];
                endTime = (BigInteger)args[1];

                // BigInteger Begintime = new BigInteger(beginTime);
                // BigInteger endtime = new BigInteger(endTime);
                //  BigInteger value1 = Neo.SmartContract.Framework.Helper.AsBigInteger(beginTime);

                Runtime.Notify("vault");
                BigInteger now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
                Runtime.Notify(now);
                Runtime.Notify("vault");
                byte[] s_Time = Storage.Get(Storage.CurrentContext, "openVault");
                Runtime.Notify("State");
                byte[] e_Time = Storage.Get(Storage.CurrentContext, "vaultSealed");
                // if (e_Time.Equals(0))
                Runtime.Notify("stop");
                if (s_Time.Equals(0) && (e_Time.Equals(0) && beginTime <= now && endTime >= now && endTime >= beginTime))
                // if (s_Time.Equals(0) && e_Time.Equals(0))//&& beginTime <= now)// && endTime> beginTime)
                {
                    Runtime.Notify("range");
                    Storage.Put(Storage.CurrentContext, "beginTime", beginTime);
                    Storage.Put(Storage.CurrentContext, "endTime", endTime);
                    Storage.Put(Storage.CurrentContext, "openVault", 1);
                    return true;
                }
            }
            if (operation == "closeVault")
            {

                BigInteger st_Time = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "openVault"));
                BigInteger en_Time = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "vaultSealed"));
                BigInteger now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
                Runtime.Notify("Start");
                if (st_Time.Equals(1) && en_Time.Equals(0))
                {
                    Runtime.Notify("Stop");

                    // BigInteger close =  new BigInteger(Storage.Get(Storage.CurrentContext, "Vault is Open")[0]) ;
                    //if(close.Equals(0))
                    Storage.Delete(Storage.CurrentContext, "openVault");

                    Storage.Put(Storage.CurrentContext, "openVault", 0);
                    return true;
                }
                return false;
            }

            if (operation == "sealVault")
            {
                BigInteger now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;

                byte[] Sealing1 = Storage.Get(Storage.CurrentContext, "vaultSealed");

                if (Sealing1.Equals(0))
                {
                    Storage.Put(Storage.CurrentContext, "endTime", now);

                    Storage.Put(Storage.CurrentContext, "VaultSealed", 1);
                    Storage.Delete(Storage.CurrentContext, "openVault");
                    Storage.Put(Storage.CurrentContext, "openVault", 0);
             

                    Runtime.Notify("Vault sealed");

                    return Storage.Get(Storage.CurrentContext, "VaultSealed");

                }
                  Runtime.Log("Vault  not Sealed");
                  return Storage.Get(Storage.CurrentContext, "VaultSealed");
            }

            if (operation == "extendVault")
            {
                BigInteger closureTime = (BigInteger)args[0];
                BigInteger Seal = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "vaultSealed"));
                BigInteger Open = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "openVault"));
                BigInteger now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
                BigInteger begtime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "beginTime"));
                Runtime.Notify("extend");
                if (Seal.Equals(0))// && begtime <= now )//&& closureTime >= begtime && Open.Equals(1))
                {
                    Runtime.Notify("extended");
                    Storage.Put(Storage.CurrentContext, "openVault", 0);
                    return true;

                }


                return false;


            }
            if (operation == "storeVault")
            {
                byte[] orderHash = (byte[])args[0];
                byte[] orderId = ((byte[])args[1]);
                BigInteger now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
                byte[] Seal = Storage.Get(Storage.CurrentContext, "VaultSealed");
                byte[] v_open = Storage.Get(Storage.CurrentContext,"openVault");

                beginTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "beginTime"));
                endTime = Neo.SmartContract.Framework.Helper.AsBigInteger(Storage.Get(Storage.CurrentContext, "endTime"));
                if (Seal.Equals(0) && v_open.Equals(1) && beginTime <= now && endTime >= now && endTime >= beginTime)
                {
                    Storage.Put(Storage.CurrentContext, orderHash.Concat(Neo.SmartContract.Framework.Helper.AsByteArray(" AAA")), "orderhashes");
                    Storage.Put(Storage.CurrentContext, orderHash, 1);
                    Storage.Put(Storage.CurrentContext, orderId, 1);
                    Runtime.Log("Well Stored");
                }
                else
                {
                    Runtime.Log("Vault Not Stored");
                }
                return true;

            }

            if (operation == "orderLocate")
            {
                byte[] hash = (byte[])args[0];
                byte[] orderId = (byte[])args[1];

                Runtime.Log("Order");
            
                return Storage.Get(Storage.CurrentContext, orderId);

            }

            if (operation == " DEx1WaySig")
            {
                byte[] baseToken = (byte[])args[0];
                byte[] etherToken = (byte[])args[1];
                if (baseToken != null && etherToken != null)
                {
                    byte[] owner = Storage.Get(Storage.CurrentContext, "approver");
                    Storage.Put(Storage.CurrentContext, owner, 1);
                    Storage.Put(Storage.CurrentContext, "b_Token", baseToken);
                    Storage.Put(Storage.CurrentContext, "e_Token", etherToken);
                }
                return false;


            }


            if (operation == "updateFeeSchedule")
            {
                uint baseTokenFee = (uint)args[0];
                uint etherTokenFee = (uint)args[1];
                uint normalTokenFee = (uint)args[2];

                if (baseTokenFee >= 0 && baseTokenFee <= 1 && etherTokenFee >= 0 && etherTokenFee <= 1 && normalTokenFee >= 0)
                {
                    Storage.Put(Storage.CurrentContext, "0", baseTokenFee);
                    Storage.Put(Storage.CurrentContext, "1", etherTokenFee);
                    Storage.Put(Storage.CurrentContext, "2", normalTokenFee);

                }
                return true;
            }

            if (operation == "killExchange")
            {
                // if (Runtime.CheckWitness(Storage.Get(Storage.CurrentContext, "approver"))) ;
                {
                    Storage.Put(Storage.CurrentContext, "contractActive", 0);
                    return true;
                }
            }

            if (operation == "isOrderSigned")
            {
                Runtime.Notify("Signature Verify");


                byte[] address = (byte[])args[0];
                byte _msgHash = (byte)args[1];
                uint v = (uint)args[2];
                byte r = (byte)args[3];
                byte s = (byte)args[4];
                byte[] _signer = (byte[])args[5];

                return Main("ecverify", address, _msgHash, v, r, s, _signer);

            }

            if (operation == "orderExists")
            {
                byte[] hash = (byte[])args[0];
                byte[] orderId = (byte[])args[1];

                if (Main("orderLocate", hash, orderId) != null)
                {
                    return true;
                }

                return false;
            }

            if (operation == "ValidExchangeFee")
                  {
                      //byte[] zero = { };
                      byte[] sellerFeeToken = (byte[])args[0];
                      byte[] buyerFeeToken = (byte[])args[1];
                      BigInteger sellerFeeValue = (BigInteger)args[2];
                      BigInteger buyerFeeValue = (BigInteger)args[3];
                      if (sellerFeeToken != null && buyerFeeToken != null && sellerFeeValue > 0 && buyerFeeValue > 0)
                      {
                          return true;

                      }
                      return false;
                  }
                  if (operation == "getFeeIndex")
                  {

                      byte[] NewToken = (byte[])args[0];
                      if (NewToken != null)
                      {
                          byte[] base_Token = Storage.Get(Storage.CurrentContext, "b_Token");
                          byte[] ether_Token = Storage.Get(Storage.CurrentContext, "e_Token");
                          if (NewToken == base_Token)
                          {
                              return 0;
                          }
                          else if (NewToken == ether_Token)
                          {
                              return 1;

                          }
                          return 2;
                      }
                      return false;
                  }
                  if (operation == "getOrderHash")
                  {
                      byte[] _tokens = (byte[])args[0];
                      uint[] _counts = (uint[])args[1];
                      uint _pValue = (uint)args[2];
                      uint _fValue = (uint)args[3];
                      byte[] _feeeToken = (byte[])args[4];
                      byte[] _maker = (byte[])args[5];
                      byte[] _signer = (byte[])args[6];
                      byte[] _orderID = (byte[])args[7];
                      //  byte[] shaHash= keccak256(byte[](this), _tokens, _counts, _pValue, _fValue, _feeeToken, _maker, _signer, _orderID);

                      return true;
                  }
          
                   if (operation == "transferTokens")
                     {
                         //orderAddress[5]= [maker,seller,buyer,seller fee token,buyer fee token]
                         byte[][] sell_Token = (byte[][])args[0];
                         byte[][] buy_Token = (byte[][])args[1];
                         BigInteger[] seller_values = (BigInteger[])args[2];
                         BigInteger[] buyer_values = (BigInteger[])args[3];
                         byte[][] order_Address = new byte[5][];
                         order_Address = (byte[][])args[4];
                         BigInteger[] order_values = new BigInteger[5];
                         order_values = (BigInteger[])args[5];
                         for (uint i = 0; i < sell_Token.Length; i++)
                         {
                             Runtime.Notify ("sell_Token");

                             Main("transferFrom", order_Address[1], order_Address[2], seller_values[i]);
                         }
                         Runtime.Notify("success");
                
                         for (uint j = 0; j < buy_Token.Length; j++)
                         {
                             Runtime.Notify("buyToken");
                             Main("transferFrom", order_Address[2], order_Address[1], buyer_values[j]);
                         }
                          Runtime.Notify("buyer Token transfeered success");
                         byte[]  wallet = Storage.Get(Storage.CurrentContext, "wallet");
                         Main("transferFrom", order_Address[1], wallet, order_values[0]);
                         Main("transferFrom", order_Address[2], wallet, order_values[1]);
                     }

                    if (operation == "transferFrom")
                    {
                    byte[] From = (byte[])args[0];
                    byte[] to = (byte[])args[1];
                    Runtime.Notify("transf");
                    BigInteger Amount = (BigInteger)args[2];
                    BigInteger FROM_Value = new BigInteger(Storage.Get(Storage.CurrentContext, From));
                    BigInteger Amount1 = FROM_Value - Amount;
                    Storage.Put(Storage.CurrentContext, From, Amount1);
                    BigInteger to1 = new BigInteger(Storage.Get(Storage.CurrentContext, to));
                    BigInteger Amount2 = to1 + Amount;
                    Storage.Put(Storage.CurrentContext, to, Amount2);
                    return true;
                    }
                  if (operation == "balanceof")
                  {
                   byte[] Acc_Addre = (byte[])args[0];
                   BigInteger balance = new BigInteger(Storage.Get(Storage.CurrentContext, Acc_Addre));
                   Runtime.Notify("success");

                return balance;
            }

            return false;


        }
    }
}






