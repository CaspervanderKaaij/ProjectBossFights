// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rift"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_DistortionMultiplier("DistortionMultiplier", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortionMultiplier;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _TextureSample1;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 uv_TexCoord1 = i.uv_texcoord * float2( 1.5,1.5 ) + float2( -0.14,-0.27 );
			float cos29 = cos( 0.81 );
			float sin29 = sin( 0.81 );
			float2 rotator29 = mul( uv_TexCoord1 - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
			float4 tex2DNode2 = tex2D( _TextureSample0, rotator29, float2( 0,0 ), float2( 0,0 ) );
			float4 lerpResult7 = lerp( ase_grabScreenPosNorm , ( _DistortionMultiplier * ( tex2DNode2.r + ase_grabScreenPosNorm ) ) , tex2DNode2.r);
			float4 screenColor9 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,lerpResult7.xy/lerpResult7.w);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			o.Emission = ( ( screenColor9 * ( 1.0 - tex2DNode2.r ) ) + ( ( 1.0 - step( tex2DNode2.r , 0.96 ) ) * tex2D( _TextureSample1, ase_screenPosNorm.xy ) ) ).rgb;
			float smoothstepResult27 = smoothstep( 0.0 , 0.25 , tex2DNode2.r);
			o.Alpha = smoothstepResult27;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
0;73;1924;928;1931.62;614.5751;1.570506;True;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-1263.414,376.4016;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;0.81;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1381.34,81.8486;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.5,1.5;False;1;FLOAT2;-0.14,-0.27;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;29;-1083.414,225.4016;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;28;-1486.566,-509.8441;Inherit;False;805.4713;447.0662;yeet;3;5;6;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GrabScreenPosition;5;-1436.566,-269.7779;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-851.5396,104.3486;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;f35cef3a157cf9844a9606139f3998fb;279697e2758c5724aad60d6dc5a0523a;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-931.0947,-459.8441;Inherit;False;Property;_DistortionMultiplier;DistortionMultiplier;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-1169.783,-378.7693;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GrabScreenPosition;8;-542.4648,-695.1095;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-680.0947,-330.8441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;12;-1052.287,687.933;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-422.4087,-247.3065;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StepOpNode;25;-531.2336,244.7218;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.96;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;22;-479.0358,128.0274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-769.2869,699.933;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;-1;3a712e4b235b5e5418a6e9162df96fa5;3a712e4b235b5e5418a6e9162df96fa5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;9;-163.3654,-193.7096;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;26;-305.066,357.2606;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-249.7463,34.52295;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-269.6509,644.717;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;3;-915.5896,372.7187;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;27;-249.0851,282.2049;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-186.6589,182.472;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;4;-499.3189,509.9152;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-257.8993,740.2384;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;39.10071,569.2384;Inherit;True;Property;_LeMask;LeMask;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Rift;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;1;0
WireConnection;29;2;30;0
WireConnection;2;1;29;0
WireConnection;6;0;2;1
WireConnection;6;1;5;0
WireConnection;18;0;19;0
WireConnection;18;1;6;0
WireConnection;7;0;8;0
WireConnection;7;1;18;0
WireConnection;7;2;2;1
WireConnection;25;0;2;1
WireConnection;22;0;2;1
WireConnection;11;1;12;0
WireConnection;9;0;7;0
WireConnection;26;0;25;0
WireConnection;20;0;9;0
WireConnection;20;1;22;0
WireConnection;13;0;26;0
WireConnection;13;1;11;0
WireConnection;3;0;2;1
WireConnection;27;0;2;1
WireConnection;21;0;20;0
WireConnection;21;1;13;0
WireConnection;4;0;3;0
WireConnection;23;1;24;0
WireConnection;0;2;21;0
WireConnection;0;9;27;0
ASEEND*/
//CHKSM=031EEFC0C2D0AC63125FD820368AF78288CBBFC0