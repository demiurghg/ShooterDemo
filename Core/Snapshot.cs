﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Core.Configuration;
using Fusion.Core.Shell;
using Fusion.Core.Utils;
using System.IO;

namespace ShooterDemo.Core {
	public class Snapshot {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		public static byte[] WriteSnapshot( EntityCollection entities )
		{
			var ents = entities.OrderBy( e => e.UniqueID ).ToArray();

			using ( var ms = new MemoryStream() ) { 
				using ( var writer = new BinaryWriter(ms) ) {

					//	write number of entities :
					writer.Write( entities.Count );

					foreach ( var ent in ents ) {
						ent.Write( writer );
					}

					return ms.GetBuffer();
				}
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entities"></param>
		public static void ReadSnapshot ( byte[] snapshot, EntityCollection entities, Action<Entity> spawn, Action<Entity> kill )
		{
			foreach ( var ent in entities ) {
				ent.Stale = true;
			}

			using ( var ms = new MemoryStream(snapshot) ) { 
				using ( var reader = new BinaryReader(ms) ) {

					//	read count :
					int count = reader.ReadInt32();

					//	read entities :
					for (int i=0; i<count; i++) {
						
						int  id		=	reader.ReadUInt32();

						var ent		=	entities[ id ];

						if (ent==null) {
							ent = new Entity();
							spawn( ent );
							entities.Add( ent );
						}

						ent.Read( reader );
					}
				}
			}
		}
	}
}