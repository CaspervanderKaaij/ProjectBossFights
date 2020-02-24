// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PocketDimension"
{
	Properties
	{
		_Voronoi("Voronoi", Vector) = (0,6.32,0,0)
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		[HDR]_Color1("Color 1", Color) = (0,0,0,0)
		_Timescale("Timescale", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
		};

		uniform float4 _Color0;
		uniform float4 _Color1;
		uniform float2 _Voronoi;
		uniform float _Timescale;


		float2 voronoihash2( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi2( float2 v, float time, inout float2 id )
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
			 		float2 o = voronoihash2( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.500 * pow( ( pow( abs( r.x ), 1 ) + pow( abs( r.y ), 1 ) ), 1.000 );
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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime9 = _Time.y * _Timescale;
			float time2 = mulTime9;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 coords2 = ase_screenPosNorm.xy * _Voronoi.y;
			float2 id2 = 0;
			float voroi2 = voronoi2( coords2, time2,id2 );
			float4 lerpResult6 = lerp( _Color0 , _Color1 , ( 1.0 - voroi2 ));
			o.Emission = lerpResult6.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
912;81;589;646;956.8668;75.47683;1.136129;True;False
Node;AmplifyShaderEditor.RangedFloatNode;10;-900.3981,349.4946;Inherit;False;Property;_Timescale;Timescale;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;1;-612.3116,29.91394;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;4;-495.7361,254.8346;Inherit;False;Property;_Voronoi;Voronoi;0;0;Create;True;0;0;False;0;0,6.32;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;9;-719.2037,379.2836;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;2;-288.7633,33.87714;Inherit;True;0;4;1;1;1;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;6.32;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.OneMinusNode;5;-36.16502,257.1826;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-379.3994,-380.3704;Inherit;False;Property;_Color0;Color 0;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-326.2626,-189.3652;Inherit;False;Property;_Color1;Color 1;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;6;-50.5257,-90.27242;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;189.5689,2.872257;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;PocketDimension;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;10;0
WireConnection;2;0;1;0
WireConnection;2;1;9;0
WireConnection;2;2;4;2
WireConnection;5;0;2;0
WireConnection;6;0;7;0
WireConnection;6;1;8;0
WireConnection;6;2;5;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=AF067DD15018C55B89A5B6D6BF6B5E940FFED01C