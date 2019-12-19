// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterForCasper"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Base("Base", Color) = (0,0.5635319,1,0)
		_Foam("Foam", Color) = (1,1,1,0)
		_FoamScale("FoamScale", Float) = 8.17
		_FoamTimeScale("FoamTimeScale", Float) = 1
		_Float0("Float 0", Float) = 0
		_WaveDirection("WaveDirection", Vector) = (0,0,0,0)
		_Float1("Float 1", Float) = 0
		_WaveHeight("WaveHeight", Float) = 1
		_WaveDirectionOffset("WaveDirectionOffset", Vector) = (0,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float3 _WaveDirectionOffset;
		uniform float _Float0;
		uniform float _Float1;
		uniform float _FoamScale;
		uniform float _FoamTimeScale;
		uniform float2 _WaveDirection;
		uniform float _WaveHeight;
		uniform float4 _Base;
		uniform float4 _Foam;
		uniform float _EdgeLength;


		float2 voronoihash1( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi1( float2 v, float time, inout float2 id )
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
			 		float2 o = voronoihash1( n + g );
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
			return F1;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float mulTime7 = _Time.y * _FoamTimeScale;
			float time1 = mulTime7;
			float2 panner22 = ( 1.0 * _Time.y * _WaveDirection + v.texcoord.xy);
			float2 coords1 = panner22 * _FoamScale;
			float2 id1 = 0;
			float fade1 = 0.5;
			float voroi1 = 0;
			float rest1 = 0;
			for( int it = 0; it <8; it++ ){
			voroi1 += fade1 * voronoi1( coords1, time1, id1 );
			rest1 += fade1;
			coords1 *= 2;
			fade1 *= 0.5;
			}
			voroi1 /= rest1;
			float smoothstepResult12 = smoothstep( _Float0 , _Float1 , voroi1);
			v.vertex.xyz += ( _WaveDirectionOffset * ( smoothstepResult12 * _WaveHeight ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime7 = _Time.y * _FoamTimeScale;
			float time1 = mulTime7;
			float2 panner22 = ( 1.0 * _Time.y * _WaveDirection + i.uv_texcoord);
			float2 coords1 = panner22 * _FoamScale;
			float2 id1 = 0;
			float fade1 = 0.5;
			float voroi1 = 0;
			float rest1 = 0;
			for( int it = 0; it <8; it++ ){
			voroi1 += fade1 * voronoi1( coords1, time1, id1 );
			rest1 += fade1;
			coords1 *= 2;
			fade1 *= 0.5;
			}
			voroi1 /= rest1;
			float smoothstepResult12 = smoothstep( _Float0 , _Float1 , voroi1);
			float4 lerpResult11 = lerp( _Base , _Foam , smoothstepResult12);
			o.Emission = lerpResult11.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
0;73;1924;928;1642.43;261.2587;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;8;-1219.115,770.4153;Inherit;False;Property;_FoamTimeScale;FoamTimeScale;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;23;-1075.992,364.9686;Inherit;False;Property;_WaveDirection;WaveDirection;11;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1004.115,228.4153;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;7;-956.1147,786.4153;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-935.1147,520.4153;Inherit;False;Property;_FoamScale;FoamScale;7;0;Create;True;0;0;False;0;8.17;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;22;-764.9923,384.9686;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-291.5914,224.2567;Inherit;False;Property;_Float0;Float 0;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;1;-565.4147,287.4153;Inherit;False;0;0;1;0;8;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;20.43;False;2;FLOAT;1;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.RangedFloatNode;13;-468.3195,877.2197;Inherit;False;Property;_Float1;Float 1;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;12;-253.5304,716.4415;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-9.19535,782.371;Inherit;False;Property;_WaveHeight;WaveHeight;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;21;298.866,303.5897;Inherit;False;Property;_WaveDirectionOffset;WaveDirectionOffset;14;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;2;-735.1147,-234.5847;Inherit;False;Property;_Base;Base;5;0;Create;True;0;0;False;0;0,0.5635319,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-724.1147,-28.58469;Inherit;False;Property;_Foam;Foam;6;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-6.136841,595.5348;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-670.1147,578.4153;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;11;-375.7627,-114.0072;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;9;-358.2652,431.8709;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.52;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;255.866,535.5897;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;WaterForCasper;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;8;0
WireConnection;22;0;4;0
WireConnection;22;2;23;0
WireConnection;1;0;22;0
WireConnection;1;1;7;0
WireConnection;1;2;5;0
WireConnection;12;0;1;0
WireConnection;12;1;10;0
WireConnection;12;2;13;0
WireConnection;19;0;12;0
WireConnection;19;1;15;0
WireConnection;11;0;2;0
WireConnection;11;1;3;0
WireConnection;11;2;12;0
WireConnection;9;0;10;0
WireConnection;9;1;1;0
WireConnection;20;0;21;0
WireConnection;20;1;19;0
WireConnection;0;2;11;0
WireConnection;0;11;20;0
ASEEND*/
//CHKSM=F9366783CBB8A4D594DA1CFFBA1E9072506A1DBE