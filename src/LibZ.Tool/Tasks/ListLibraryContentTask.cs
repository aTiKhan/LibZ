﻿#region License

/*
 * Copyright (c) 2013-2014, Milosz Krajewski
 * 
 * Microsoft Public License (Ms-PL)
 * This license governs use of the accompanying software. 
 * If you use the software, you accept this license. 
 * If you do not accept the license, do not use the software.
 * 
 * 1. Definitions
 * The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same 
 * meaning here as under U.S. copyright law.
 * A "contribution" is the original software, or any additions or changes to the software.
 * A "contributor" is any person that distributes its contribution under this license.
 * "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 * 
 * 2. Grant of Rights
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions 
 * and limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
 * royalty-free copyright license to reproduce its contribution, prepare derivative works of 
 * its contribution, and distribute its contribution or any derivative works that you create.
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and 
 * limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
 * royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, 
 * import, and/or otherwise dispose of its contribution in the software or derivative works of 
 * the contribution in the software.
 * 
 * 3. Conditions and Limitations
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, 
 * logo, or trademarks.
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed 
 * by the software, your patent license from such contributor to the software ends automatically.
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, 
 * and attribution notices that are present in the software.
 * (D) If you distribute any portion of the software in source code form, you may do so only under this 
 * license by including a complete copy of this license with your distribution. If you distribute 
 * any portion of the software in compiled or object code form, you may only do so under a license 
 * that complies with this license.
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express
 * warranties, guarantees or conditions. You may have additional consumer rights under your local 
 * laws which this license cannot change. To the extent permitted under your local laws, the 
 * contributors exclude the implied warranties of merchantability, fitness for a particular 
 * purpose and non-infringement.
 */

#endregion

using System.Collections.Generic;
using System.Linq;
using LibZ.Manager;
using LibZ.Manager.Internal;
using NLog;

namespace LibZ.Tool.Tasks
{
	/// <summary>Lists content of .libz container.</summary>
	public class ListLibraryContentTask: TaskBase
	{
		#region consts

		/// <summary>Logger for this class.</summary>
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		#endregion

		/// <summary>Executes the task.</summary>
		/// <param name="libzFileName">Name of the libz file.</param>
		public virtual void Execute(string libzFileName)
		{
			Log.Info("Opening '{0}'", libzFileName);
			using (var container = new LibZContainer(libzFileName))
			{
				var orderedEnties = container.Entries
					.OrderBy(e => e.AssemblyName.Name)
					.ThenBy(e => e.AssemblyName.Version);

				foreach (var entry in orderedEnties)
				{
					var ratio = entry.OriginalLength != 0
						? entry.StorageLength * 100 / entry.OriginalLength
						: 100;

					Log.Info(entry.AssemblyName.FullName);
					Log.Debug("    flags:{0}, codec:'{1}', size:{2}, compession:{3}%, id:{4}",
						string.Join("|", GetFlagsText(entry.Flags)),
						entry.CodecName ?? "<none>",
						entry.OriginalLength,
						ratio,
						entry.Id.ToString("N"));
				}
			}
		}

		/// <summary>Gets the flags text.</summary>
		/// <param name="entryFlags">The entry flags.</param>
		/// <returns>Text representing assembly flags.</returns>
		private static IEnumerable<string> GetFlagsText(LibZEntry.EntryFlags entryFlags)
		{
			if ((entryFlags & LibZEntry.EntryFlags.Unmanaged) != 0)
				yield return "Unmanaged";

			if ((entryFlags & LibZEntry.EntryFlags.AnyCPU) != 0)
				yield return "AnyCPU";
			else if ((entryFlags & LibZEntry.EntryFlags.X64) != 0)
				yield return "x64";
			else
				yield return "x86";

			if ((entryFlags & LibZEntry.EntryFlags.Portable) != 0)
				yield return "Portable";
			if ((entryFlags & LibZEntry.EntryFlags.SafeLoad) != 0)
				yield return "SafeLoad";
		}
	}
}