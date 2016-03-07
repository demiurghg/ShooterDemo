using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Input;

namespace ShooterDemo {
	public class ShooterClientConfig {

		public float Sensitivity { get; set; }
		public bool InvertMouse { get; set; }
		public float PullFactor { get; set; }
		public bool ThirdPerson { get; set; }

		public float ZoomFov { get; set; }
		public float Fov { get; set; }
		public float BobHeave	{ get; set; }
		public float BobPitch	{ get; set; }
		public float BobRoll	{ get; set; }
		public float BobStrafe  { get; set; }
		public float BobJump	{ get; set; }
		public float BobLand	{ get; set; }


		public Keys	MoveForward		{ get; set; }
		public Keys	MoveBackward	{ get; set; }
		public Keys	StrafeRight		{ get; set; }
		public Keys	StrafeLeft		{ get; set; }
		public Keys	Jump			{ get; set; }
		public Keys Crouch			{ get; set; }
		public Keys Walk			{ get; set; }

		public Keys Attack			{ get; set; }
		public Keys Zoom			{ get; set; }
		public Keys ThrowGrenade	{ get; set; }

		public Keys UseWeapon1		{ get; set; }
		public Keys UseWeapon2		{ get; set; }
		public Keys UseWeapon3		{ get; set; }
		public Keys UseWeapon4		{ get; set; }
		public Keys UseWeapon5		{ get; set; }
		public Keys UseWeapon6		{ get; set; }
		public Keys UseWeapon7		{ get; set; }
		public Keys UseWeapon8		{ get; set; }
		public Keys UseWeapon9		{ get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ShooterClientConfig ()
		{
			Sensitivity	=	5;
			InvertMouse	=	false;
			PullFactor	=	1;

			Fov			=	90.0f;
			ZoomFov		=	30.0f;

			BobHeave	=	0.05f;
			BobPitch	=	1.0f;
			BobRoll		=	2.0f;
			BobStrafe  	=	5.0f;
			BobJump		=	5.0f;
			BobLand		=	5.0f;


			MoveForward		=	Keys.S;
			MoveBackward	=	Keys.Z;
			StrafeRight		=	Keys.X;
			StrafeLeft		=	Keys.A;
			Jump			=	Keys.RightButton;
			Crouch			=	Keys.LeftAlt;
			Walk			=	Keys.LeftShift;
							
			Attack			=	Keys.LeftButton;
			Zoom			=	Keys.D;
			ThrowGrenade	=	Keys.G;
								
			UseWeapon1		=	Keys.D1;
			UseWeapon2		=	Keys.D2;
			UseWeapon3		=	Keys.D3;
			UseWeapon4		=	Keys.D4;
			UseWeapon5		=	Keys.D5;
			UseWeapon6		=	Keys.D6;
			UseWeapon7		=	Keys.D7;
			UseWeapon8		=	Keys.D8;
			UseWeapon9		=	Keys.D9;
		}
	}
}
