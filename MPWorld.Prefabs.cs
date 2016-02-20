using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Content;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;
using ShooterDemo.Views;
using ShooterDemo.Controllers;

namespace ShooterDemo {
	public partial class MPWorld : World {


		/// <summary>
		/// 
		/// </summary>
		void InitializePrefabs ()
		{
			AddController( new Characters(this, PhysSpace) );

			AddView( new ModelView(this) );
			AddView( new CameraView(this) );


			AddPrefab( "startPoint"	, PrefabDummy	);
			AddPrefab( "player"		, PrefabPlayer	);
		}



		/*-----------------------------------------------------------------------------------------
		 * 
		 *	PREFABS :
		 * 
		-----------------------------------------------------------------------------------------*/

		/// <summary>
		/// Used for entities that are just locators, like starting points, etc.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="e"></param>
		public static void PrefabDummy ( World w, Entity e )
		{
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="serverSide"></param>
		public static void PrefabPlayer ( World world, Entity entity )
		{
			world.GetController<Characters>().AddCharacter( entity );

			if (world.IsClientSide) {
			}
		}

	}
}
