// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Remasu/Barrier"
{
	Properties
	{
		[HDR]_BarrierGlow("BarrierGlow", Color) = (0,0.225235,1.828908,0)
		_OpacityTest("OpacityTest", Float) = 1
		_Timescale("Timescale", Float) = 1
		_Float1("Float 1", Float) = 1
		[HDR]_BarrierBaseColor("Barrier Base Color", Color) = (1,0,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_GradientLinesScaleDotCom("GradientLinesScaleDotCom", Float) = 1
		_LineDirection("LineDirection", Vector) = (0,-1,0,0)
		_LinesSpeedScale("LinesSpeedScale", Float) = 1
		_LineStrength("LineStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _BarrierBaseColor;
		uniform float4 _BarrierGlow;
		uniform float _Float1;
		uniform float _Timescale;
		uniform float _OpacityTest;
		uniform sampler2D _TextureSample1;
		uniform float _LinesSpeedScale;
		uniform float2 _LineDirection;
		uniform float _GradientLinesScaleDotCom;
		uniform float _LineStrength;


		float2 voronoihash3( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi3( float2 v, float time, inout float2 id )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash3( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F2;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime9 = _Time.y * _Timescale;
			float time3 = mulTime9;
			float2 coords3 = i.uv_texcoord * _Float1;
			float2 id3 = 0;
			float fade3 = 0.5;
			float voroi3 = 0;
			float rest3 = 0;
			for( int it = 0; it <7; it++ ){
			voroi3 += fade3 * voronoi3( coords3, time3, id3 );
			rest3 += fade3;
			coords3 *= 2;
			fade3 *= 0.5;
			}
			voroi3 /= rest3;
			float temp_output_16_0 = ( voroi3 * _OpacityTest );
			float4 lerpResult21 = lerp( _BarrierBaseColor , _BarrierGlow , temp_output_16_0);
			o.Albedo = lerpResult21.rgb;
			float mulTime28 = _Time.y * _LinesSpeedScale;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 panner26 = ( mulTime28 * _LineDirection + (ase_screenPosNorm*_GradientLinesScaleDotCom + 0.0).xy);
			float4 tex2DNode23 = tex2D( _TextureSample1, panner26 );
			o.Alpha = min( ( temp_output_16_0 * tex2DNode23 ) , ( tex2DNode23 * _LineStrength ) ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
905;81;665;640;764.7189;-210.6828;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;10;-1549.377,574.2566;Inherit;False;Property;_Timescale;Timescale;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;22;-976.1622,735.3224;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-1227.162,997.3224;Inherit;False;Property;_GradientLinesScaleDotCom;GradientLinesScaleDotCom;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1006.972,1366.531;Inherit;False;Property;_LinesSpeedScale;LinesSpeedScale;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1343.977,396.7694;Inherit;False;Property;_Float1;Float 1;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;9;-1331.111,644.4176;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1272.569,185.4481;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;24;-869.1622,953.3224;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;28;-776.59,1365.439;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;27;-910.1622,1159.323;Inherit;False;Property;_LineDirection;LineDirection;9;0;Create;True;0;0;False;0;0,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.VoronoiNode;3;-964.308,407.9875;Inherit;False;0;0;1;1;7;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0.18;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.PannerNode;26;-660.1622,1113.323;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-529.48,524.3132;Inherit;False;Property;_OpacityTest;OpacityTest;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-327.5553,417.8333;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;23;-445.1622,854.3224;Inherit;True;Property;_TextureSample1;Texture Sample 1;7;0;Create;True;0;0;False;0;-1;05aa43dbbeddb24419e5b5b6e734d595;05aa43dbbeddb24419e5b5b6e734d595;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-278.4423,1099.17;Inherit;False;Property;_LineStrength;LineStrength;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-259.0544,705.4244;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;-456.9617,-209.1216;Inherit;False;Property;_BarrierBaseColor;Barrier Base Color;6;1;[HDR];Create;True;0;0;False;0;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-469.8046,-40.30344;Inherit;False;Property;_BarrierGlow;BarrierGlow;0;1;[HDR];Create;True;0;0;False;0;0,0.225235,1.828908,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-402.6005,672.9994;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-932.4362,124.7271;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;5a302fa48864ff9498fd3b7508f66b6e;5a302fa48864ff9498fd3b7508f66b6e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;-151.9617,89.87842;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-597.0771,229.242;Inherit;False;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;33;-181.2953,565.3547;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;18;-24.18133,295.2679;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Remasu/Barrier;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;10;0
WireConnection;24;0;22;0
WireConnection;24;1;25;0
WireConnection;28;0;29;0
WireConnection;3;0;1;0
WireConnection;3;1;9;0
WireConnection;3;2;14;0
WireConnection;26;0;24;0
WireConnection;26;2;27;0
WireConnection;26;1;28;0
WireConnection;16;0;3;0
WireConnection;16;1;17;0
WireConnection;23;1;26;0
WireConnection;31;0;23;0
WireConnection;31;1;32;0
WireConnection;30;0;16;0
WireConnection;30;1;23;0
WireConnection;21;0;20;0
WireConnection;21;1;11;0
WireConnection;21;2;16;0
WireConnection;33;0;30;0
WireConnection;33;1;31;0
WireConnection;18;0;21;0
WireConnection;18;9;33;0
ASEEND*/
//CHKSM=276227140E1ED2499A4349347819743907D2E2B2