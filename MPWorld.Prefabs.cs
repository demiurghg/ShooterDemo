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
			AddView( new ModelView(this) );
			AddView( new SfxView(this) );
			AddView( new CameraView(this) );
			AddView( new HudView(this) );


			AddPrefab( "startPoint"	, PrefabDummy	);
			AddPrefab( "camera"		, PrefabDummy	);
			AddPrefab( "player"		, PrefabPlayer	);
			AddPrefab( "box"		, PrefabBox		);
			AddPrefab( "rocket"		, PrefabRocket	);
			AddPrefab( "explosion"	, PrefabExplosion );
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
			entity.Attach( new Characters( entity, world ) );
			entity.Attach( new Weaponry( entity, world ) );

			if (world.IsClientSide) {
				world.GetView<ModelView>().AddModel( entity, @"scenes\characters\marine\marine", "marine", Matrix.Scaling(0.1f) * Matrix.RotationY(MathUtil.Pi), Matrix.Translation(0,-0.85f,0) );
			}

			entity.SetItemCount( Inventory.Health	,	100	);
			entity.SetItemCount( Inventory.Armor	,	0	);
		}



		public static void PrefabRocket ( World world, Entity entity )
		{
			entity.Attach( new Projectiles( entity, world, "Explosion", 30, 5, 100, 100, 5 ) );
			
			if (world.IsClientSide) {
				world.GetView<ModelView>().AddModel( entity, @"scenes\weapon\projRocket", "rocket", Matrix.Scaling(0.1f), Matrix.Identity );
				world.GetView<SfxView>().AttachSFX ( entity, "RocketTrail" );
			}
		}



		public static void PrefabExplosion ( World world, Entity entity )
		{
		}



		public static void PrefabBox ( World world, Entity entity )
		{
			entity.Attach( new RigidBody(entity, world, 1.0f, 0.75f, 0.75f,5 ) ); 

			if (world.IsClientSide) {
				world.GetView<ModelView>().AddModel( entity, @"scenes\boxes\boxModel", "pCube1", Matrix.Identity, Matrix.Identity );
			}
		}

	}
}
