using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Engine.Graphics;
using Fusion.Engine.Common;
using Fusion.Engine.Server;
using Fusion.Engine.Client;
using BEPUVector3 = BEPUutilities.Vector3;
using BEPUTransform = BEPUutilities.AffineTransform;
using Fusion.Core.Content;
using ShooterDemo.Entities;

namespace ShooterDemo {
	public partial class GameWorld {

		List<MeshInstance>	instances = new List<MeshInstance>();


		/// <summary>
		/// Gets list of static instances.
		/// </summary>
		public ICollection<MeshInstance> StaticMeshInstances {
			get { return instances; }
		}



		/// <summary>
		/// We assume that scene represents level.
		/// </summary>
		/// <param name="scene"></param>
		void ReadMapFromScene ( ContentManager content, Scene scene, bool createRendMeshes )
		{
			//	create entity list :
			entities	=	new EntityCollection(this);
			instances	=	new List<MeshInstance>();

			//	compute absolute transforms :
			var transforms	= new Matrix[ scene.Nodes.Count ];
			scene.ComputeAbsoluteTransforms( transforms );
			var materials	= new MaterialInstance[0];

			if (createRendMeshes) {
				var defMtrl	=	content.Game.RenderSystem.DefaultMaterial;
				materials	=	scene.Materials.Select( m => content.Load<MaterialInstance>( m.Name, defMtrl ) ).ToArray();
			}


			//	iterate through the scene's nodes :
			for ( int i=0; i<scene.Nodes.Count; i++) {

				var node	=	scene.Nodes[ i ];
				var world	=	transforms[ i ];
				var name	=	node.Name;
				var mesh	=	node.MeshIndex < 0 ? null : scene.Meshes[ node.MeshIndex ];


				if (name.StartsWith("startPoint")) {	
					entities.Add( new StartPoint( node, world ) );
					continue;
				}


				if (mesh!=null) {
					AddStaticMesh( mesh, world );

					if (createRendMeshes) {
						var mi		= new MeshInstance( content.Game.RenderSystem, scene, mesh, materials );
						mi.World	= world;

						instances.Add( mi );
					}
				}
			}
		}
	}
}
