// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BlackholeDistort"
{
	Properties
	{
		_FresnelPower("FresnelPower", Float) = 0.09247717
		_m("m", Float) = 0.56

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Overlay" }
	LOD 413

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Off
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		GrabPass{ }

		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
			};

			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform float _FresnelPower;
			uniform float _m;
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
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float3 ase_worldPos = i.ase_texcoord.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode1 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV1, _FresnelPower ) );
				float temp_output_11_0 = ( 1.0 - fresnelNode1 );
				float3 objToViewDir6 = mul( UNITY_MATRIX_IT_MV, float4( -i.ase_normal, 0 ) ).xyz;
				float4 screenPos = i.ase_texcoord2;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 screenColor3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float4( ( temp_output_11_0 * (objToViewDir6).xyz ) , 0.0 ) + ase_grabScreenPos ).xy/( float4( ( temp_output_11_0 * (objToViewDir6).xyz ) , 0.0 ) + ase_grabScreenPos ).w);
				float temp_output_21_0 = ( 1.0 - ( temp_output_11_0 * _m ) );
				float clampResult19 = clamp( temp_output_21_0 , 0.0 , 1.0 );
				
				
				finalColor = ( screenColor3 * clampResult19 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17400
911;73;594;651;622.2548;-457.6254;1;False;False
Node;AmplifyShaderEditor.NormalVertexDataNode;4;-1284.989,272.5631;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2;-886.8314,62.07962;Inherit;False;Property;_FresnelPower;FresnelPower;0;0;Create;True;0;0;False;0;0.09247717;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;5;-1100.389,310.5631;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;1;-600.1405,-25.31886;Inherit;True;Standard;WorldNormal;ViewDir;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;6;-967.989,400.5631;Inherit;False;Object;View;False;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;11;-258.5558,37.89273;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-887.6924,726.2892;Inherit;False;Property;_m;m;1;0;Create;True;0;0;False;0;0.56;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;7;-736.989,278.5631;Inherit;False;True;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-700.2645,763.4991;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;13;-689.5572,469.7706;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-472.0269,250.7821;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-419.0609,393.8536;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;21;-632.3593,867.7142;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;3;-265.3888,300.2628;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;19;-260.0305,803.4297;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;14;-1038.185,184.4243;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;16.07513,463.9086;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;18;-455.8822,778.9484;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;15;0,0;Float;False;True;-1;2;ASEMaterialInspector;413;1;BlackholeDistort;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Overlay=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;5;0;4;0
WireConnection;1;3;2;0
WireConnection;6;0;5;0
WireConnection;11;0;1;0
WireConnection;7;0;6;0
WireConnection;17;0;11;0
WireConnection;17;1;16;0
WireConnection;10;0;11;0
WireConnection;10;1;7;0
WireConnection;9;0;10;0
WireConnection;9;1;13;0
WireConnection;21;0;17;0
WireConnection;3;0;9;0
WireConnection;19;0;21;0
WireConnection;14;0;4;0
WireConnection;20;0;3;0
WireConnection;20;1;19;0
WireConnection;18;0;21;0
WireConnection;15;0;20;0
ASEEND*/
//CHKSM=9A6DFEC11D026C8E6220828F5550BC4E884CA377