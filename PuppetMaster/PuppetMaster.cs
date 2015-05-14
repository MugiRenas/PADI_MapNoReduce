﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Reflection;


using Worker_JobTracker;
using PADI_MapNoReduce;

/// PUPPETMASTER: Rui

namespace PuppetMaster
{
  class PuppetMaster : MarshalByRefObject
  {
    // int: Worker ID
    // Object: Worker instance
    private Dictionary<int, PADI_MapNoReduce.Worker> workerList = new Dictionary<int, PADI_MapNoReduce.Worker>();
    private int currentWorkerID = 0;
    private int ID;
    private int numberPM = 0;

    // Functions

    // SubmitJob
    // Submit Jobs to worker processes
    public void SubmitJob(string entryURL, string inputPath,
                          string outputPath, int nSplits,
                          string mapClassName, string mapClassPath
      ) 
    {
      // Show Message for testing - remove later
      System.Windows.Forms.MessageBox.Show("Entry URL: " + entryURL + " Input Path: " + inputPath + " Output Path: " + outputPath +
                                            " Number of Splits: " + nSplits + " Map Class Name: " + mapClassName + " Map Class Path: " + mapClassPath);


      Worker worker = (Worker)Activator.GetObject(typeof(Worker), entryURL);
      string txt = System.IO.File.ReadAllText(@inputPath);
      byte[] code = System.IO.File.ReadAllBytes(mapClassPath);

      object ClassObj = null;

      Assembly assembly = Assembly.Load(code);
      // Walk through each type in the assembly looking for our class
      foreach (Type type in assembly.GetTypes())
      {
        if (type.IsClass == true)
        {
          if (type.FullName.EndsWith("." + mapClassName))
          {
            // create an instance of the object
            ClassObj = Activator.CreateInstance(type);
          }
        }
      }

      if (ClassObj == null)
      {
        // Throw Exception
      }

    }

    // CreateWorker: 
    // Launch new Worker process
    public void CreateWorker(string workerURL, int id = 0, string entryURL = null)
    {

      // Parse worker URL
      char[] urlDelimiterChars = {':' , '/'};
      string[] parsedURL = workerURL.Split(urlDelimiterChars);
      int workerPort = Int32.Parse(parsedURL[2]);

      // Show Message for testing - remove later
      System.Windows.Forms.MessageBox.Show("ID: " + id + " Port: " + workerPort);

      // Create new worker instance and store it in array/dictionary
      if (id <= 0 && !workerList.ContainsKey(currentWorkerID))
      {
        Worker w = new Worker(workerURL, currentWorkerID.ToString());
        workerList.Add(currentWorkerID, w);
        currentWorkerID++;
      }
      else if (!workerList.ContainsKey(id))
      {
        Worker w = new Worker(workerURL, id.ToString());
        workerList.Add(id, w);
        currentWorkerID = id;
      }
      else
      {
        // THROW ERROR/EXCEPTION
      }
    }

    // Wait: 
    // Stops execution of other command scripts
    public void Wait(int time) 
    {
      // Puts system to sleep
      System.Threading.Thread.Sleep(time);
    }

    // StatusReport:
    // Requests all workers/job trackers submit/print their current status
    public void StatusReport()
    {
      string result = "";
      for (int i = 1; i <= numberPM; i++)
      {
        int portN = 20000 + i;
        if (portN != ID)
        {
          PuppetMaster pm = (PuppetMaster)Activator.GetObject(typeof(PuppetMaster), "tcp://localhost:" + portN + "/PM");
          result += pm.Report() + '\n';
        }
        else
        {
          result += Report() + '\n';
        }
      }
      System.Windows.Forms.MessageBox.Show(result);
    }

    public string Report()
    {
      // Add status requests to worker nodes
      return (ID + ": Nothing to Report");
    }

    // SlowWorker:
    // Delays (puts to sleep) a worker process
    public void SlowWorker(int workerID, int ms)
    {
      Worker w;
      if (workerList.TryGetValue(workerID, out w))
      {

      }
    }

    // FreezeWorker:
    // Disables a given worker process's Worker Functions
    public void FreezeWorker(int workerID)
    {
      Worker w;
      if(workerList.TryGetValue(workerID, out w))
      {
        if (w.Equals(w))
        {

        }
      }
    }

    // UnfreezeWorker:
    // Re-enables a given worker process's Worker Functions
    public void UnfreezeWorker(int workerID)
    {
      Worker w;
      if (workerList.TryGetValue(workerID, out w))
      {
        if (w.Equals(w))
        {

        }
      }
    }

    // FreezeJobTracker:
    // Disables a given worker process's JobTracker Functions
    public void FreezeJobTracker(int workerID)
    {
      Worker w;
      if (workerList.TryGetValue(workerID, out w))
      {
        if (w.Equals(w))
        {

        }
      }
    }

    // UnfreezeJobTracker:
    // Re-enables a given worker process's JobTracker Functions
    public void UnfreezeJobTracker(int workerID)
    {
      Worker w;
      if (workerList.TryGetValue(workerID, out w))
      {
        if (w.Equals(w))
        {

        }
      }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    //[STAThread]
    void Main(string[] args)
    {
      numberPM = Int32.Parse(args[0]);

      ID = Int32.Parse(args[1]);
      TcpChannel channel = new TcpChannel(ID);

      ChannelServices.RegisterChannel(channel, false);

      PuppetMaster pm = new PuppetMaster();
      RemotingServices.Marshal(pm, "PM", typeof(PuppetMaster));

        /*RemotingConfiguration.RegisterWellKnownServiceType(
              typeof(PuppetMaster),
              "PM",
              WellKnownObjectMode.Singleton
          );*/

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new PuppetMasterForm());
    }
  }
}