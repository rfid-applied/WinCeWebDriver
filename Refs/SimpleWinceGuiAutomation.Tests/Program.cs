using System;
using System.IO;
using NUnitLite.Runner;

namespace SimpleWinceGuiAutomation.Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
            var x = new OpenNETCF.Windows.Forms.Test.SendKeysTest();
            x.Init();
            try
            {
                x.SendKeysTestPositive();
            }
            finally
            {
                x.KillApp();
            }*/

            var l = new LexingTest();
            l.Init();
            l.Ident();
            l.TestWhiteSpaceParenIdent();
            l.KillApp();

            var p = new ParsingTest();
            p.Init();
            p.ParseIdentAttrib();
            p.ParseDescendant();
            p.ParseNext();
            p.KillApp();

            var t = new EvalTest();
            t.SuccessorTests();
            t.SimpleTagName();
            t.SuccessorTagName();

            var writer = new StringWriter();
            new TextUI(writer).Execute(args);
            if (writer.ToString().Contains("Errors and Failures"))
            {
                throw new Exception(writer.ToString());
            }
        }
    }
}