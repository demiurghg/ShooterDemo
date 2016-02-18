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


namespace ShooterDemo.Views {
	public class ModelView : EntityView {

		GameClient gameClient;

		Dictionary<uint, MeshInstance> instances = new Dictionary<uint,MeshInstance>();
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public ModelView ( GameClient gameClient, World world ) : base(world)
		{
			this.gameClient	=	gameClient;
		}


		/// <summary>
		/// Hot reload?
		/// </summary>
		/// <param name="scenePath"></param>
		/// <param name="nodeName"></param>
		/// <param name="preTransform"></param>
		public void AddModel ( Entity entity, string scenePath, string nodeName, Matrix preTransform )
		{
			var rs		= Game.RenderSystem;	  
			var content	= gameClient.Content;

			var scene = content.Load<Scene>( scenePath );
			var node  = scene.Nodes.FirstOrDefault( n => n.Name == nodeName );

			if (node==null) {
				Log.Warning("Scene '{0}' does not contain node '{1}'", scenePath, nodeName );
				return;
			}

			if (node.MeshIndex<0) {
				Log.Warning("Node '{0}|{1}' does not contain mesh", scenePath, nodeName );
				return;
			}

			var mesh	=	scene.Meshes[node.MeshIndex];

			var defMtrl		=	rs.DefaultMaterial;
			var materials	=	scene.Materials.Select( m => content.Load<MaterialInstance>( m.Name, defMtrl ) ).ToArray();

			var instance = new MeshInstance( rs, scene, mesh, materials );

			instances.Add( entity.UniqueID, instance ); 

			Game.RenderSystem.RenderWorld.Instances.Add( instance );
		} 



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Present ( GameTime gameTime )
		{
			foreach ( var item in instances ) {
				var ent =	World.GetEntity( item.Key );
				var wm	=	ent.GetWorldMatrix();

				item.Value.World = wm;
				item.Value.Visible = true;
			}
		}



		/// <summary>
		/// Removes entity
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{	
			MeshInstance instance;

			if (instances.TryGetValue( id, out instance )) {
				Game.RenderSystem.RenderWorld.Instances.Remove( instance );
				instances.Remove( id );
			} else {
				Log.Warning("ModelView: can not remove entity #{0}", id );
			}
		}

	}
}
