// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MikaShieldo"
{
	Properties
	{
		_NoiseMovement("NoiseMovement", Vector) = (0,0,0,0)
		_CellSpeed("CellSpeed", Float) = 1
		_CellScale("CellScale", Float) = 1
		_H("H", Float) = 0
		_NoiseTiling("NoiseTiling", Vector) = (0,0,0,0)
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		[HDR]_Color1("Color 1", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color0;
		uniform float4 _Color1;
		uniform float _CellScale;
		uniform float _CellSpeed;
		uniform float2 _NoiseMovement;
		uniform float2 _NoiseTiling;
		uniform float _H;


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
			for ( int j = -2; j <= 2; j++ )
			{
				for ( int i = -2; i <= 2; i++ )
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
			
F1 = 8.0;
for ( int j = -2; j <= 2; j++ )
{
for ( int i = -2; i <= 2; i++ )
{
float2 g = mg + float2( i, j );
float2 o = voronoihash1( n + g );
		o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
float d = dot( 0.5 * ( mr + r ), normalize( r - mr ) );
F1 = min( F1, d );
}
}
return F1;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult22 = lerp( _Color0 , _Color1 , i.uv_texcoord.y);
			o.Emission = lerpResult22.rgb;
			float mulTime3 = _Time.y * _CellSpeed;
			float time1 = mulTime3;
			float2 uv_TexCoord2 = i.uv_texcoord * _NoiseTiling;
			float2 panner5 = ( 1.0 * _Time.y * _NoiseMovement + uv_TexCoord2);
			float2 coords1 = panner5 * _CellScale;
			float2 id1 = 0;
			float voroi1 = voronoi1( coords1, time1,id1 );
			float clampResult14 = clamp( ( _H + i.uv_texcoord.y ) , 0.0 , 1.0 );
			o.Alpha = ( 1.0 - step( voroi1 , clampResult14 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
816;73;755;656;1218.042;280.3583;1;True;False
Node;AmplifyShaderEditor.Vector2Node;19;-1685.426,152.1432;Inherit;False;Property;_NoiseTiling;NoiseTiling;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;4;-1441.132,19.05316;Inherit;False;Property;_CellSpeed;CellSpeed;1;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;8;-1455.132,277.0532;Inherit;False;Property;_NoiseMovement;NoiseMovement;0;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;17;-1149.507,549.0211;Inherit;False;Property;_H;H;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1496.364,126.696;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1322.84,668.679;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1059.507,673.0211;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1193.43,340.6002;Inherit;False;Property;_CellScale;CellScale;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;5;-1224.132,197.0532;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-1259.132,26.05316;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;14;-962.5074,776.0211;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;1;-1027.976,144.8563;Inherit;True;1;0;1;4;1;False;0;False;3;0;FLOAT2;0,0;False;1;FLOAT;5.64;False;2;FLOAT;7.04;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.StepOpNode;12;-784.123,645.1152;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-975.0416,-87.35825;Inherit;False;Property;_Color1;Color 1;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;20;-966.4834,-399.8173;Inherit;False;Property;_Color0;Color 0;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-989.8069,-219.6469;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;18;-738.6882,567.0552;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;22;-709.0416,-167.3582;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-709.2504,81.71088;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;MikaShieldo;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;19;0
WireConnection;16;0;17;0
WireConnection;16;1;10;2
WireConnection;5;0;2;0
WireConnection;5;2;8;0
WireConnection;3;0;4;0
WireConnection;14;0;16;0
WireConnection;1;0;5;0
WireConnection;1;1;3;0
WireConnection;1;2;9;0
WireConnection;12;0;1;0
WireConnection;12;1;14;0
WireConnection;18;0;12;0
WireConnection;22;0;20;0
WireConnection;22;1;23;0
WireConnection;22;2;21;2
WireConnection;0;2;22;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=D524E4F20CFB48893EDD40800CAF030AB0312AD8