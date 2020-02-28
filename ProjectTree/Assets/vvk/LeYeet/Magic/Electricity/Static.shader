// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Static"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_EleSpeedScale("EleSpeedScale", Float) = 1
		_NoisePanning("NoisePanning", Vector) = (0,1,1,0)
		_yeet("yeet", Float) = 0
		_Witdth("Witdth", Float) = 3

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _EleSpeedScale;
				uniform float4 _NoisePanning;
				uniform float _yeet;
				uniform float _Witdth;
				float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
				float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
				float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
				float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
				float snoise( float3 v )
				{
					const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
					float3 i = floor( v + dot( v, C.yyy ) );
					float3 x0 = v - i + dot( i, C.xxx );
					float3 g = step( x0.yzx, x0.xyz );
					float3 l = 1.0 - g;
					float3 i1 = min( g.xyz, l.zxy );
					float3 i2 = max( g.xyz, l.zxy );
					float3 x1 = x0 - i1 + C.xxx;
					float3 x2 = x0 - i2 + C.yyy;
					float3 x3 = x0 - 0.5;
					i = mod3D289( i);
					float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
					float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
					float4 x_ = floor( j / 7.0 );
					float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
					float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
					float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
					float4 h = 1.0 - abs( x ) - abs( y );
					float4 b0 = float4( x.xy, y.xy );
					float4 b1 = float4( x.zw, y.zw );
					float4 s0 = floor( b0 ) * 2.0 + 1.0;
					float4 s1 = floor( b1 ) * 2.0 + 1.0;
					float4 sh = -step( h, 0.0 );
					float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
					float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
					float3 g0 = float3( a0.xy, h.x );
					float3 g1 = float3( a0.zw, h.y );
					float3 g2 = float3( a1.xy, h.z );
					float3 g3 = float3( a1.zw, h.w );
					float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
					g0 *= norm.x;
					g1 *= norm.y;
					g2 *= norm.z;
					g3 *= norm.w;
					float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
					m = m* m;
					m = m* m;
					float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
					return 42.0 * dot( m, px);
				}
				


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.ase_texcoord3.xyz = ase_worldPos;
					
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float mulTime15 = _Time.y * _EleSpeedScale;
					float2 appendResult17 = (float2(_NoisePanning.x , _NoisePanning.y));
					float3 ase_worldPos = i.ase_texcoord3.xyz;
					float2 panner12 = ( mulTime15 * appendResult17 + ase_worldPos.xy);
					float simplePerlin3D11 = snoise( float3( panner12 ,  0.0 )*3.79 );
					simplePerlin3D11 = simplePerlin3D11*0.5 + 0.5;
					float2 appendResult18 = (float2(_NoisePanning.z , _NoisePanning.w));
					float2 panner13 = ( mulTime15 * appendResult18 + ase_worldPos.xy);
					float simplePerlin3D1 = snoise( float3( panner13 ,  0.0 )*_yeet );
					simplePerlin3D1 = simplePerlin3D1*0.5 + 0.5;
					float2 temp_cast_4 = ((-_Witdth + (pow( ( simplePerlin3D11 + simplePerlin3D1 ) , 13.55 ) - 0.0) * (_Witdth - -_Witdth) / (1.0 - 0.0))).xx;
					float2 appendResult10_g2 = (float2(0.5 , 0.5));
					float2 temp_output_11_0_g2 = ( abs( (temp_cast_4*2.0 + -1.0) ) - appendResult10_g2 );
					float2 break16_g2 = ( 1.0 - ( temp_output_11_0_g2 / fwidth( temp_output_11_0_g2 ) ) );
					float4 temp_cast_5 = (saturate( min( break16_g2.x , break16_g2.y ) )).xxxx;
					

					fixed4 col = temp_cast_5;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17700
595;81;926;640;62.84668;485.7381;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;14;-1332.317,196.1917;Inherit;False;Property;_EleSpeedScale;EleSpeedScale;0;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;16;-1136.317,-269.8083;Inherit;False;Property;_NoisePanning;NoisePanning;1;0;Create;True;0;0;False;0;0,1,1,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;15;-1127.317,225.1917;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-891.3165,-366.8083;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;-1025.317,78.19171;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-1216.2,-60.59998;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;13;-860.3165,8.191704;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-866.7045,227.284;Inherit;False;Property;_yeet;yeet;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;12;-821.3165,-127.8083;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;11;-584.3165,-134.8083;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;3.79;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-691,114.5;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;3.79;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-353.368,86.01065;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-297.1615,380.5916;Inherit;False;Property;_Witdth;Witdth;3;0;Create;True;0;0;False;0;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;22;-157.368,83.01065;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;13.55;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;24;-142.1615,334.5916;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;6;84.2,283.7;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-3;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;20;-511.5294,-425.9392;Inherit;True;Polar Coordinates;-1;;4;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;9;210.761,28.80314;Inherit;True;Rectangle;-1;;2;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;296.1533,-321.7381;Inherit;False;Property;_Color0;Color 0;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;10;666.2,-129.8;Float;False;True;-1;2;ASEMaterialInspector;0;7;Static;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;15;0;14;0
WireConnection;17;0;16;1
WireConnection;17;1;16;2
WireConnection;18;0;16;3
WireConnection;18;1;16;4
WireConnection;13;0;3;0
WireConnection;13;2;18;0
WireConnection;13;1;15;0
WireConnection;12;0;3;0
WireConnection;12;2;17;0
WireConnection;12;1;15;0
WireConnection;11;0;12;0
WireConnection;1;0;13;0
WireConnection;1;1;23;0
WireConnection;21;0;11;0
WireConnection;21;1;1;0
WireConnection;22;0;21;0
WireConnection;24;0;25;0
WireConnection;6;0;22;0
WireConnection;6;3;24;0
WireConnection;6;4;25;0
WireConnection;9;1;6;0
WireConnection;10;0;9;0
ASEEND*/
//CHKSM=8F00976B12BE56126448FF1BF52933CEB4BBB854