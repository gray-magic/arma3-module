using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Morrigan
{
    public class DllEntry
    {



        public static ExtensionCallback callback;
        public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);
        [DllExport("RVExtensionRegisterCallback", CallingConvention = CallingConvention.Winapi)]
        public static void RVExtensionRegisterCallback([MarshalAs(UnmanagedType.FunctionPtr)] ExtensionCallback func)
        {
            try
            {
                callback = func;
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Gets called when arma starts up and loads all extension.
        /// It's perfect to load in static objects in a seperate thread so that the extension doesn't needs any seperate initalization
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            try
            {
                output.Append("Morrigan v1.1");

                //new Thread(() =>
                //{


                //}).Start();
            }
            catch (Exception)
            {
                //RAS
            }

        }

        /// <summary>
        /// The entry point for the default callExtension command.
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
        /// <param name="function">The string argument that is used along with callExtension</param>
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
        public static void RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            try
            {
                Orders_Functions Order = new Orders_Functions(function,null);
                Order.Auto_Execute(output);
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// The entry point for the callExtensionArgs command.
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
        /// <param name="function">The string argument that is used along with callExtension</param>
        /// <param name="args">The args passed to callExtension as a string array</param>
        /// <param name="argsCount">The size of the string array args</param>
        /// <returns>The result code</returns>

        [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
        public static int RvExtensionArgs(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
        {
            try
            {
                Orders_Functions Order = new Orders_Functions(function, args);
                return Order.Auto_Execute(output);
            }
            catch (Exception)
            {
                return 0;
            }

        }



    }
}


