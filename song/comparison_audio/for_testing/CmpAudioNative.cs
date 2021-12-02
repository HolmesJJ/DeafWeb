/*
* MATLAB Compiler: 6.6 (R2018a)
* Date: Mon Dec 24 18:22:25 2018
* Arguments:
* "-B""macro_default""-W""dotnet:comparison_audio,CmpAudio,4.0,private""-T""link:lib""-d""
* C:\Users\HJJ\Downloads\song\comparison_audio\for_testing""-v""class{CmpAudio:C:\Users\HJ
* J\Downloads\song\comparison_audio.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace comparison_audioNative
{

  /// <summary>
  /// The CmpAudio class provides a CLS compliant, Object (native) interface to the
  /// MATLAB functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\HJJ\Downloads\song\comparison_audio.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class CmpAudio : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static CmpAudio()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "comparison_audio.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the CmpAudio class.
    /// </summary>
    public CmpAudio()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~CmpAudio()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object comparison_audio()
    {
      return mcr.EvaluateFunction("comparison_audio", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="d1_path">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object comparison_audio(Object d1_path)
    {
      return mcr.EvaluateFunction("comparison_audio", d1_path);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="d1_path">Input argument #1</param>
    /// <param name="d2_path">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object comparison_audio(Object d1_path, Object d2_path)
    {
      return mcr.EvaluateFunction("comparison_audio", d1_path, d2_path);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] comparison_audio(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "comparison_audio", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="d1_path">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] comparison_audio(int numArgsOut, Object d1_path)
    {
      return mcr.EvaluateFunction(numArgsOut, "comparison_audio", d1_path);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the comparison_audio MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="d1_path">Input argument #1</param>
    /// <param name="d2_path">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] comparison_audio(int numArgsOut, Object d1_path, Object d2_path)
    {
      return mcr.EvaluateFunction(numArgsOut, "comparison_audio", d1_path, d2_path);
    }


    /// <summary>
    /// Provides an interface for the comparison_audio function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// 此处显示有关此函数的摘要
    /// 此处显示详细说明
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("comparison_audio", 2, 6, 0)]
    protected void comparison_audio(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("comparison_audio", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }

    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
