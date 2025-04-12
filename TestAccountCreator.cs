//****************************************
// Copyright (c) Thinkability Group 2011
// SystemsAdminPro.com
//****************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.DirectoryServices;
using Microsoft.Win32;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Reflection;


namespace TestAccountCreator
{
    class TACMain
    {
        struct CMDArguments
        {
            public bool bParseCmdArguments;
            public string strPrincipalContextType;
            public string strTestAccountName;
            public int intNumOfTestAccounts;
            public string strTestAccountPassword;
        }

        static bool funcLicenseCheck()
        {
            string strLicenseString = "";
            bool bValidLicense = false;

            try
            {
                TextReader tr = new StreamReader("sotfwlic.dat");

                try
                {
                    strLicenseString = tr.ReadLine();

                    if (strLicenseString.Length > 0 & strLicenseString.Length < 29)
                    {
                        // [DebugLine] Console.WriteLine("if: " + strLicenseString);
                        Console.WriteLine("Invalid license");

                        tr.Close(); // close license file

                        return bValidLicense;
                    }
                    else
                    {
                        tr.Close(); // close license file
                        // [DebugLine] Console.WriteLine("else: " + strLicenseString);

                        string strMonthTemp = ""; // to convert the month into the proper number
                        string strDate;

                        //Month
                        strMonthTemp = strLicenseString.Substring(7, 1);
                        if (strMonthTemp == "A")
                        {
                            strMonthTemp = "10";
                        }
                        if (strMonthTemp == "B")
                        {
                            strMonthTemp = "11";
                        }
                        if (strMonthTemp == "C")
                        {
                            strMonthTemp = "12";
                        }
                        strDate = strMonthTemp;

                        //Day
                        strDate = strDate + "/" + strLicenseString.Substring(16, 1);
                        strDate = strDate + strLicenseString.Substring(6, 1);

                        // Year
                        strDate = strDate + "/" + strLicenseString.Substring(24, 1);
                        strDate = strDate + strLicenseString.Substring(4, 1);
                        strDate = strDate + strLicenseString.Substring(1, 2);

                        // [DebugLine] Console.WriteLine(strDate);
                        // [DebugLine] Console.WriteLine(DateTime.Today.ToString());
                        DateTime dtLicenseDate = DateTime.Parse(strDate);
                        // [DebugLine]Console.WriteLine(dtLicenseDate.ToString());

                        if (dtLicenseDate >= DateTime.Today)
                        {
                            bValidLicense = true;
                        }
                        else
                        {
                            Console.WriteLine("License expired.");
                        }

                        return bValidLicense;
                    }

                } //end of try block on tr.ReadLine

                catch
                {
                    // [DebugLine] Console.WriteLine("catch on tr.Readline");
                    Console.WriteLine("Invalid license");
                    tr.Close();
                    return bValidLicense;

                } //end of catch block on tr.ReadLine

            } // end of try block on new StreamReader("sotfwlic.dat")

            catch (System.Exception ex)
            {
                // [DebugLine] System.Console.WriteLine("{0} exception caught here.", ex.GetType().ToString());

                // [DebugLine] System.Console.WriteLine(ex.Message);

                if (ex.Message.StartsWith("Could not find file"))
                {
                    Console.WriteLine("License file not found.");
                }
                else
                {
                    Console.WriteLine("tacf1: {0}", ex.Message);
                }

                return bValidLicense;

            } // end of catch block on new StreamReader("sotfwlic.dat")
        }

        static bool funcLicenseActivation()
        {
            try
            {
                if (funcCheckForFile("TurboActivate.dll"))
                {
                    if (funcCheckForFile("TurboActivate.dat"))
                    {
                        TurboActivate.VersionGUID = "4935355894e0da3d4465e86.37472852";

                        if (TurboActivate.IsActivated())
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("A license for this product has not been activated.");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("TurboActivate.dat is required and could not be found.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("TurboActivate.dll is required and could not be found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                return false;
            }
        }

        static void funcPrintParameterWarning()
        {
            Console.WriteLine("Parameters must be specified properly to run TestAccountCreator.");
            Console.WriteLine("Run TestAccountCreator -? to get the parameter syntax.");
        }

        static void funcPrintParameterSyntax()
        {
            Console.WriteLine("TestAccountCreator v1.0 (c) 2011 SystemsAdminPro.com");
            Console.WriteLine();
            Console.WriteLine("Parameter syntax:");
            Console.WriteLine();
            Console.WriteLine("Use the following required parameters in the following order:");
            Console.WriteLine("-run                required parameter");
            Console.WriteLine("-name:              to specify the name for test account");
            Console.WriteLine("-num:               to specify the number of test accounts to create");
            Console.WriteLine("-pass:              to specify the password for test accounts");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("TestAccountCreator -run -name:TestUser -num:100 -pass:TestPass00");
        }

        static CMDArguments funcParseCmdArguments(string[] cmdargs)
        {
            CMDArguments objCMDArguments = new CMDArguments();

            try
            {
                objCMDArguments.strPrincipalContextType = "";
                bool bCmdArg1Complete = false;
                bool bCmdArg2Complete = false;

                if (cmdargs[0] == "-run" & cmdargs.Length > 1)
                {
                    if (cmdargs[1].Contains("-name:"))
                    {
                        // [DebugLine] Console.WriteLine(cmdargs[1].Substring(6));
                        objCMDArguments.strTestAccountName = cmdargs[1].Substring(6);
                        bCmdArg1Complete = true;

                        if (bCmdArg1Complete & cmdargs.Length > 2)
                        {
                            if (cmdargs[2].Contains("-num:"))
                            {
                                // [DebugLine] Console.WriteLine(cmdargs[2].Substring(5));
                                objCMDArguments.intNumOfTestAccounts = Int32.Parse(cmdargs[2].Substring(5));
                                bCmdArg2Complete = true;

                                if (bCmdArg2Complete & cmdargs.Length > 3)
                                {
                                    if (cmdargs[3].Contains("-pass:"))
                                    {
                                        // [DebugLine] Console.WriteLine(cmdargs[3].Substring(6));
                                        objCMDArguments.strTestAccountPassword = cmdargs[3].Substring(6);
                                        objCMDArguments.bParseCmdArguments = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    objCMDArguments.bParseCmdArguments = false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            return objCMDArguments;
        }

        static void funcProgramExecution(CMDArguments objCMDArguments2)
        {
            try
            {
                // [DebugLine] Console.WriteLine("Entering funcProgramExecution");

                int i = 0;
                string newUserName = "";

                funcProgramRegistryTag("TestAccountCreator");

                i = objCMDArguments2.intNumOfTestAccounts;
                newUserName = objCMDArguments2.strTestAccountName;

                //Console.WriteLine(i.ToString());

                DirectorySearcher tempDS = funcCreateDSSearcher();

                // [DebugLine] Console.WriteLine(tempDS.SearchRoot.Path);

                string strCreateUPN = tempDS.SearchRoot.Path;

                strCreateUPN = strCreateUPN.Replace(",DC=", ".");

                strCreateUPN = strCreateUPN.Replace("LDAP://DC=", "@");

                // [DebugLine] Console.WriteLine(strCreateUPN);

                PrincipalContext ctx = funcCreatePrincipalContext(objCMDArguments2.strPrincipalContextType);

                if (ctx != null)
                {
                    for (int j = 1; j <= i; j++)
                    {
                        UserPrincipal userNew = new UserPrincipal(ctx);
                        if (userNew != null)
                        {
                            userNew.SamAccountName = newUserName + j.ToString();
                            userNew.Name = newUserName + j.ToString();
                            userNew.DisplayName = newUserName + j.ToString();
                            userNew.UserPrincipalName = newUserName + j.ToString() + strCreateUPN;
                            userNew.SetPassword(objCMDArguments2.strTestAccountPassword);
                            userNew.Enabled = true;
                            userNew.Save();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No valid PrincipalContext.");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("80071392"))
                {
                    Console.WriteLine("Check Active Directory for the account(s).");
                }
                else
                {
                    MethodBase mb1 = MethodBase.GetCurrentMethod();
                    funcGetFuncCatchCode(mb1.Name, ex);
                }
            }

        }

        static void funcProgramRegistryTag(string strProgramName)
        {
            try
            {
                string strRegistryProfilesPath = "SOFTWARE";
                RegistryKey objRootKey = Microsoft.Win32.Registry.LocalMachine;
                RegistryKey objSoftwareKey = objRootKey.OpenSubKey(strRegistryProfilesPath, true);
                RegistryKey objSystemsAdminProKey = objSoftwareKey.OpenSubKey("SystemsAdminPro", true);
                if (objSystemsAdminProKey == null)
                {
                    objSystemsAdminProKey = objSoftwareKey.CreateSubKey("SystemsAdminPro");
                }
                if (objSystemsAdminProKey != null)
                {
                    if (objSystemsAdminProKey.GetValue(strProgramName) == null)
                        objSystemsAdminProKey.SetValue(strProgramName, "1", RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }
        }

        static DirectorySearcher funcCreateDSSearcher()
        {
            System.DirectoryServices.DirectorySearcher objDSSearcher = new DirectorySearcher();
            // [Comment] Get local domain context
            try
            {
                string rootDSE;

                System.DirectoryServices.DirectorySearcher objrootDSESearcher = new System.DirectoryServices.DirectorySearcher();
                rootDSE = objrootDSESearcher.SearchRoot.Path;
                //Console.WriteLine(rootDSE);

                // [Comment] Construct DirectorySearcher object using rootDSE string
                System.DirectoryServices.DirectoryEntry objrootDSEentry = new System.DirectoryServices.DirectoryEntry(rootDSE);
                objDSSearcher = new System.DirectoryServices.DirectorySearcher(objrootDSEentry);
                //Console.WriteLine(objDSSearcher.SearchRoot.Path);
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            return objDSSearcher;
        }

        static PrincipalContext funcCreatePrincipalContext(string strContextType)
        {
            PrincipalContext newctx = new PrincipalContext(ContextType.Machine);

            try
            {
                //Console.WriteLine("Entering funcCreatePrincipalContext");
                Domain objDomain = Domain.GetComputerDomain();
                string strDomain = objDomain.Name;
                DirectorySearcher tempDS = funcCreateDSSearcher();
                string strDomainRoot = "CN=Users," + tempDS.SearchRoot.Path.Substring(7);
                // [DebugLine] Console.WriteLine(strDomainRoot);
                // [DebugLine] Console.WriteLine(strDomainRoot);

                newctx = new PrincipalContext(ContextType.Domain,
                                    strDomain,
                                    strDomainRoot);

                // [DebugLine] Console.WriteLine(newctx.ConnectedServer);
                // [DebugLine] Console.WriteLine(newctx.Container);



                //if (strContextType == "Domain")
                //{

                //    PrincipalContext newctx = new PrincipalContext(ContextType.Domain,
                //                                    strDomain,
                //                                    strDomainRoot);
                //    return newctx;
                //}
                //else
                //{
                //    PrincipalContext newctx = new PrincipalContext(ContextType.Machine);
                //    return newctx;
                //}
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            if (newctx.ContextType == ContextType.Machine)
            {
                Console.WriteLine("tacf8: The Active Directory context did not initialize properly.");
            }

            return newctx;
        }

        static void funcGetFuncCatchCode(string strFunctionName, Exception currentex)
        {
            string strCatchCode = "";

            Dictionary<string, string> dCatchTable = new Dictionary<string, string>();
            dCatchTable.Add("funcGetFuncCatchCode", "f0");
            dCatchTable.Add("funcLicenseCheck", "f1");
            dCatchTable.Add("funcPrintParameterWarning", "f2");
            dCatchTable.Add("funcPrintParameterSyntax", "f3");
            dCatchTable.Add("funcParseCmdArguments", "f4");
            dCatchTable.Add("funcProgramExecution", "f5");
            dCatchTable.Add("funcProgramRegistryTag", "f6");
            dCatchTable.Add("funcCreateDSSearcher", "f7");
            dCatchTable.Add("funcCreatePrincipalContext", "f8");
            dCatchTable.Add("funcCheckNameExclusion", "f9");
            dCatchTable.Add("funcMoveDisabledAccounts", "f10");
            dCatchTable.Add("funcFindAccountsToDisable", "f11");
            dCatchTable.Add("funcCheckLastLogin", "f12");
            dCatchTable.Add("funcRemoveUserFromGroup", "f13");
            dCatchTable.Add("funcToEventLog", "f14");
            dCatchTable.Add("funcCheckForFile", "f15");
            dCatchTable.Add("funcCheckForOU", "f16");
            dCatchTable.Add("funcWriteToErrorLog", "f17");
            dCatchTable.Add("funcGetUserGroups", "f18");
            dCatchTable.Add("funcLicenseActivation", "f19");
            dCatchTable.Add("funcOpenOutputLog", "f20");
            dCatchTable.Add("funcWriteToOutputLog", "f21");
            dCatchTable.Add("funcSearchForUser", "f22");
            dCatchTable.Add("funcSearchForGroup", "f23");
            dCatchTable.Add("funcGetGroup", "f24");
            dCatchTable.Add("funcGetUser", "f25");
            dCatchTable.Add("funcParseUserName", "f26");
            dCatchTable.Add("funcAddUserToGroup", "f27");
            dCatchTable.Add("funcGetColumnSelection", "f28");
            dCatchTable.Add("funcPrintColumnSelect", "f29");
            dCatchTable.Add("funcProcessFiles", "f30");
            dCatchTable.Add("funcCheckFileRowsForDelimiter", "f31");
            dCatchTable.Add("funcSysQueryData", "f32");
            dCatchTable.Add("funcSysQueryData2", "f33");
            dCatchTable.Add("funcGetProfileData", "f34");
            dCatchTable.Add("funcCheckOSCaptionVersion", "f35");
            dCatchTable.Add("funcRemoveProfile", "f36");
            dCatchTable.Add("funcRecurse", "f37");
            dCatchTable.Add("funcRemoveDirectory", "f38");

            if (dCatchTable.ContainsKey(strFunctionName))
            {
                strCatchCode = "err" + dCatchTable[strFunctionName] + ": ";
            }

            //[DebugLine] Console.WriteLine(strCatchCode + currentex.GetType().ToString());
            //[DebugLine] Console.WriteLine(strCatchCode + currentex.Message);

            funcWriteToErrorLog(strCatchCode + currentex.GetType().ToString());
            funcWriteToErrorLog(strCatchCode + currentex.Message);

        }

        static void funcWriteToErrorLog(string strErrorMessage)
        {
            try
            {
                FileStream newFileStream = new FileStream("Err-TestAccountCreator.log", FileMode.Append, FileAccess.Write);
                TextWriter twErrorLog = new StreamWriter(newFileStream);

                DateTime dtNow = DateTime.Now;

                string dtFormat = "MMddyyyy HH:mm:ss";

                twErrorLog.WriteLine("{0} \t {1}", dtNow.ToString(dtFormat), strErrorMessage);

                twErrorLog.Close();
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static bool funcCheckForFile(string strInputFileName)
        {
            try
            {
                if (System.IO.File.Exists(strInputFileName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                return false;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                //if (funcLicenseCheck())
                if (funcLicenseActivation())
                {
                    if (args.Length == 0)
                    {
                        funcPrintParameterWarning();
                    }
                    else
                    {
                        if (args[0] == "-?")
                        {
                            funcPrintParameterSyntax();
                        }
                        else
                        {
                            string[] arrArgs = args;
                            CMDArguments objArgumentsProcessed = funcParseCmdArguments(arrArgs);

                            if (objArgumentsProcessed.bParseCmdArguments)
                            {
                                funcProgramExecution(objArgumentsProcessed);
                            }
                            else
                            {
                                funcPrintParameterWarning();
                            } // check objArgumentsProcessed.bParseCmdArguments
                        } // check args[0] = "-?"
                    } // check args.Length == 0
                } // funcLicenseCheck()
            }
            catch (Exception ex)
            {
                Console.WriteLine("tacf9: {0}", ex.Message);
            }

        } // Main()
    }
}
