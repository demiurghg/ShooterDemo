using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Core.Content;
using Fusion.Engine.Server;
using Fusion.Engine.Client;

namespace ShooterDemo.Core {

	/// <summary>
	/// World represents entire game state.
	/// </summary>
	public abstract class World {

		public readonly Game Game;

		public readonly bool serverSide;

		public readonly ContentManager Content;

		const uint MaxEntities = 1024;
		Entity[]	entities;
		uint counter	=	MaxEntities - 1;

		List<EntityView> views = new List<EntityView>();
		List<EntityController> controllers = new List<EntityController>();

		Dictionary<uint, Prefab> prefabs = new Dictionary<uint,Prefab>();


		class Prefab {
			public string Name;
			public Action<Entity,bool> Construct;
		}


		/// <summary>
		/// Gets entities.
		/// </summary>
		public Entity[] Entities {
			get { return entities; }
		}


		/// <summary>
		/// Indicates that world is running on server side.
		/// </summary>
		public bool IsServer {
			get { return serverSide; }
		}



		/// <summary>
		/// Indicates that world is running on client side.
		/// </summary>
		public bool IsClient {
			get { return !serverSide; }
		}



		/// <summary>
		/// Initializes server-side world.
		/// </summary>
		/// <param name="maxPlayers"></param>
		/// <param name="maxEntities"></param>
		public World ( GameServer server )
		{
			this.serverSide	=	true;
			this.Game		=	server.Game;
			Content			=	server.Content;
			entities		=	new Entity[ MaxEntities ];
		}



		/// <summary>
		/// Initializes client-side world.
		/// </summary>
		/// <param name="client"></param>
		public World ( GameClient client )
		{
			this.serverSide	=	false;
			this.Game		=	client.Game;
			Content			=	client.Content;
			entities		=	new Entity[ MaxEntities ];
		}



		/// <summary>
		/// Adds view.
		/// </summary>
		/// <param name="view"></param>
		public void AddView( EntityView view )
		{
			if (IsServer) {
				throw new InvalidOperationException("Can not add EntityView to server-side world");
			}
			views.Add( view );
		}



		/// <summary>
		/// Adds controller.
		/// </summary>
		/// <param name="controller"></param>
		public void AddController ( EntityController controller )
		{
			controllers.Add( controller );
		}



		/// <summary>
		/// Gets view by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetView<T>() where T: EntityView 
		{
			return (T)views.FirstOrDefault( v => v is T );
		}



		/// <summary>
		/// Gets controller by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetController<T>() where T: EntityController 
		{
			return (T)controllers.FirstOrDefault( c => c is T );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabName"></param>
		/// <param name="constructAction"></param>
		public void AddPrefab ( string prefabName, Action<Entity,bool> constructAction )
		{
			uint crc = Factory.GetPrefabID( prefabName );

			if (prefabs.ContainsKey(crc)) {
				throw new ArgumentException("Prefab '" + prefabName + "' cause hash collision with + '" + prefabs[crc].Name + "'");
			}

			prefabs.Add( crc, new Prefab(){ Name = prefabName, Construct = constructAction } );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabId"></param>
		void Construct ( Entity entity )
		{
			prefabs[ entity.PrefabID ].Construct(entity, IsServer);
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		void Destruct ( Entity entity )
		{
			foreach ( var controller in controllers ) {
				controller.Kill( entity.UniqueID );
			}
			foreach ( var view in views ) {
				view.Kill( entity.UniqueID );
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update ( GameTime gameTime )
		{
			//
			//	Control entities :
			//
			foreach ( var controller in controllers ) {
				controller.Update( gameTime );
			}

			//
			//	Present entities :
			//
			if (IsClient) {
				foreach ( var view in views ) {
					view.Present( gameTime );
				}
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		/// <param name="origin"></param>
		/// <param name="angles"></param>
		/// <returns></returns>
		public uint Spawn ( string prefab, uint parentId, Vector3 origin, Angles angles )
		{
			uint prefabId = Factory.GetPrefabID( prefab );


			//	search for empty entity slot :
			for (uint i=0; i<(uint)MaxEntities; i++) {

				//	increase counter
				counter++;

				//	skip zero value:
				if (counter==0) counter=1;

				uint index = counter % MaxEntities;

				if (entities[i].UniqueID==0) {

					entities[i].Initialize( prefabId, counter, parentId, origin, angles );

					return counter;
				}
			}


			Log.Warning("Can not spawn entity: {0}", prefab );

			return 0;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public uint GetIndex ( uint id )
		{
			if (id==0) {
				throw new ArgumentException("Entity ID is zero", "id");
			}

			return id % MaxEntities;
		}



		/// <summary>
		/// Check whether entity with id is dead.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool IsAlive ( uint id )
		{
			return entities[ GetIndex(id) ].UniqueID != 0;
		}



		/// <summary>
		/// Gets entity with current id.
		/// If entity is dead -> exception...
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Entity GetEntity ( uint id )
		{
			if (!IsAlive(id)) {
				throw new InvalidOperationException(string.Format("Entity #{0} is dead", id));
			}
			return entities[ GetIndex(id) ];
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Kill ( uint id )
		{
			if (id==0) {
				return;
			}

			entities[ GetIndex(id) ].Kill();
		}




		/// <summary>
		/// Called when player enetered.
		/// </summary>
		/// <param name="guid"></param>
		public abstract void PlayerEntered ( Guid guid );

		/// <summary>
		/// Called when player left.
		/// </summary>
		/// <param name="guid"></param>
		public abstract void PlayerLeft ( Guid guid );


		/// <summary>
		/// Called when server started.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="mapName"></param>
		//public abstract void LoadMapServer ( ContentManager content, string mapName );


		/// <summary>
		/// Called when client started.
		/// Server info is provided.
		/// This method could be called in separate thread.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="mapName"></param>
		//public abstract void LoadMapClient ( ContentManager content, string serverInfo );


		/// <summary>
		/// This method called in main thread to complete non-thread safe operations.
		/// </summary>
		public abstract void FinalizeLoad (); 


		/// <summary>
		/// Returns server info.
		/// </summary>
		public abstract string ServerInfo (); 


		/// <summary>
		/// Called when client or server is 
		/// </summary>
		public abstract void Cleanup ();
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Write ( BinaryWriter writer )
		{
			for ( int i=0; i<MaxEntities; i++ ) {
				entities[i].Write( writer );
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Read ( BinaryReader reader )
		{
			for ( int i=0; i<MaxEntities; i++ ) {

				//	track spawn/kill on remoter server side
				var oldId = entities[i].UniqueID;

				entities[i].Read( reader );

				var newId = entities[i].UniqueID;

				if (oldId != newId) {

					if (newId!=0) {
						Construct( entities[i] );
					} else {
						Destruct( entities[i] );
					}

				}
			}
		}
	}
}
