
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PowerMedia.Common.Diagnostics
{

public static class ProcessUtils
{
    public static void KillAllProcessesWithNameLike(string name)
    {
        var processes = Process.GetProcessesByName(name);
        foreach (var process in processes)
        {
           KillProcessTree(process);
        }
    }

    //TODO: add selft check = were all the processed killed ?
    public static void KillProcessTree(Process root)
    {
	
        // Retrieve all processes on the system
        Process[] processes = Process.GetProcesses();
        foreach (Process p in processes)
        {
            // Get some basic information about the process
            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
            uint bytesWritten;
            NtQueryInformationProcess(p.Handle,
                                          0, ref pbi, (uint)Marshal.SizeOf(pbi),
                                          out bytesWritten); // == 0 is OK

            // Is it a child process of the process we're trying to terminate?
            if (pbi.InheritedFromUniqueProcessId == root.Id)
            {
                    // The terminate the child process and its child processes
                    KillProcessTree(p);
            }
        }
        try
        {
        	TerminateProcess((uint)root.Id, 0);
        }
        catch(Win32Exception){}
        
    }

    private struct PROCESS_BASIC_INFORMATION
    {
        public int ExitStatus;
        public int PebBaseAddress;
        public int AffinityMask;
        public int BasePriority;
        public uint UniqueProcessId;
        public uint InheritedFromUniqueProcessId;
    }

    [DllImport("kernel32.dll")]
    static extern bool TerminateProcess(uint hProcess, int exitCode);

    [DllImport("ntdll.dll")]
    static extern int NtQueryInformationProcess(
        IntPtr hProcess,
        int processInformationClass /* 0 */,
        ref PROCESS_BASIC_INFORMATION processBasicInformation,
        uint processInformationLength,
        out uint returnLength
    );



}
}
