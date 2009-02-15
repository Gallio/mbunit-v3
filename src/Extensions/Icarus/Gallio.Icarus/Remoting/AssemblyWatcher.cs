// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Gallio.Icarus.Remoting
{
	/// <summary>
	/// AssemblyWatcher keeps track of one or more assemblies to 
	/// see if they have changed. It incorporates a delayed notification
	/// and uses a standard event to notify any interested parties
	/// about the change. The path to the assembly is provided as
	/// an argument to the event handler so that one routine can
	/// be used to handle events from multiple watchers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Based on code taken from NUnit.
	/// <code>
	/// /************************************************************************************
	/// '
	/// ' Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
	/// ' Copyright  2000-2002 Philip A. Craig
	/// '
	/// ' This software is provided 'as-is', without any express or implied warranty. In no 
	/// ' event will the authors be held liable for any damages arising from the use of this 
	/// ' software.
	/// ' 
	/// ' Permission is granted to anyone to use this software for any purpose, including 
	/// ' commercial applications, and to alter it and redistribute it freely, subject to the 
	/// ' following restrictions:
	/// '
	/// ' 1. The origin of this software must not be misrepresented; you must not claim that 
	/// ' you wrote the original software. If you use this software in a product, an 
	/// ' acknowledgment (see the following) in the product documentation is required.
	/// '
	/// ' Portions Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
	/// ' or Copyright  2000-2002 Philip A. Craig
	/// '
	/// ' 2. Altered source versions must be plainly marked as such, and must not be 
	/// ' misrepresented as being the original software.
	/// '
	/// ' 3. This notice may not be removed or altered from any source distribution.
	/// '
	/// '***********************************************************************************/
	/// </code>
	/// </para>
	/// </remarks>
	public class AssemblyWatcher
	{
		private readonly Dictionary<string, FileWatcher> fileWatchers = new Dictionary<string, FileWatcher>();

		protected Timer timer;
		protected string changedAssemblyPath; 

		public delegate void AssemblyChangedHandler(String fullPath);
		public event AssemblyChangedHandler AssemblyChangedEvent;

		public AssemblyWatcher()
		{
			timer = new Timer(1000);
			timer.AutoReset = false;
			timer.Enabled = false;
			timer.Elapsed += OnTimer;
		}

        public void Add(IList<string> files)
        {
            foreach (string file in files)
                Add(file);
        }

        public void Add(string filePath)
        {
            if (fileWatchers.ContainsKey(filePath))
                return;
            
            FileWatcher fw = new FileWatcher(new FileInfo(filePath));

            fw.Watcher.Path = fw.Info.DirectoryName;
            fw.Watcher.Filter = fw.Info.Name;
            fw.Watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite;
            fw.Watcher.Changed += OnChanged;
            fw.Watcher.EnableRaisingEvents = false;

            fileWatchers.Add(filePath, fw);

            if (fileWatchers.Count > 0)
                Start();
        }

	    public void Remove(string filePath)
		{
            if (fileWatchers.ContainsKey(filePath))
                fileWatchers.Remove(filePath);
		}

		public void Clear()
		{
			Stop();
			foreach(string key in fileWatchers.Keys)
			{
                FileWatcher fw = fileWatchers[key];
				fw.Watcher.Changed -= OnChanged;
			}
			fileWatchers.Clear();
		}

		public void Start()
		{
			EnableWatchers(true);
		}

		public void Stop()
		{
			EnableWatchers(false);
		}

		private void EnableWatchers(bool enable)
		{
            foreach (string key in fileWatchers.Keys)
            {
                FileWatcher watcher = fileWatchers[key];
                watcher.Watcher.EnableRaisingEvents = enable;
            }
		}

		protected void OnTimer(Object source, ElapsedEventArgs e)
		{
			lock(this)
			{
				PublishEvent();
				timer.Enabled=false;
			}
		}
		
		protected void OnChanged(object source, FileSystemEventArgs e)
		{
			changedAssemblyPath = e.FullPath;
			if ( timer != null )
			{
				lock(this)
				{
					if(!timer.Enabled)
						timer.Enabled=true;
					timer.Start();
				}
			}
			else
			{
				PublishEvent();
			}
		}
	
		protected void PublishEvent()
		{
			if (AssemblyChangedEvent != null)
				AssemblyChangedEvent(changedAssemblyPath);
		}

		public class FileWatcher
		{
			private readonly FileSystemWatcher watcher = new FileSystemWatcher();
			private readonly FileInfo info;

			public FileWatcher(FileInfo info)
			{
				this.info = info;
			}

			public FileSystemWatcher Watcher
			{
				get
				{
					return watcher;
				}
			}

			public FileInfo Info
			{
				get
				{
					return info;
				}
			}
		}
	}
}