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
	public class ModelView : EntityView<ModelView.Model> {

		
		public class Model {
			public Matrix PreTransform;
			public Matrix PostTransform;
			public MeshInstance Instance;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public ModelView ( World world ) : base(world)
		{
		}


		/// <summary>
		/// Hot reload?
		/// </summary>
		/// <param name="scenePath"></param>
		/// <param name="nodeName"></param>
		/// <param name="preTransform"></param>
		public void AddModel ( Entity entity, string scenePath, string nodeName, Matrix preTransform, Matrix postTransform )
		{
			var rs		= Game.RenderSystem;	  
			var content	= World.Content;

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

			AddObject( entity.ID, new Model{ Instance = instance, PreTransform = preTransform, PostTransform = postTransform } ); 

			Game.RenderSystem.RenderWorld.Instances.Add( instance );
		} 



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( GameTime gameTime )
		{
			IterateObjects( (e,m) => {
				m.Instance.World	=	m.PreTransform * e.GetWorldMatrix() * m.PostTransform;
				m.Instance.Visible	=	e.UserGuid != World.GameClient.Guid;
			});
		}



		/// <summary>
		/// Removes entity
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{	
			Model model;

			if ( RemoveObject( id, out model ) ) {
				Game.RenderSystem.RenderWorld.Instances.Remove( model.Instance );
			}
		}

	}
}
