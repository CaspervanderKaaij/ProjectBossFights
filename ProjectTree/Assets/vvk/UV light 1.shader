// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/UV Light Reveal"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BaseTexture("Base Texture", 2D) = "white" {}
		_UVTexture("UV Texture", 2D) = "white" {}
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.4039216,0,1,1)
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
		uniform sampler2D _UVTexture;
		uniform float4 _UVTexture_ST;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;
		uniform float _Cutoff = 0.5;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_BaseTexture = i.uv_texcoord * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_59_0 = ( ase_lightAtten * ase_lightColor );
			float2 uv_UVTexture = i.uv_texcoord * _UVTexture_ST.xy + _UVTexture_ST.zw;
			float4 tex2DNode52 = tex2D( _UVTexture, uv_UVTexture );
			float3 normalizeResult116 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult117 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult96 = dot( normalizeResult116 , normalizeResult117 );
			c.rgb = 0;
			c.a = 1;
			clip( ( ( ( tex2D( _BaseTexture, uv_BaseTexture ) * saturate( temp_output_59_0 ) ) + ( tex2DNode52.r * ( ( temp_output_59_0 * _WorldSpaceLightPos0.w ) *  ( dotResult96 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult96 - _DifferenceThreshold <= 1.0 && dotResult96 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  ) ) ) * tex2DNode52.r ).r - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 color119 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			o.Emission = color119.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
8;81;1908;912;107.9163;369.3615;1;True;False
Node;AmplifyShaderEditor.WorldSpaceLightPos;54;-1219.7,205.2679;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;84;-1132.925,359.0915;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-915.9946,345.3203;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;87;-1057.309,513.7495;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;3;0;Create;True;0;0;False;0;0.4039216,0,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;92;-734.5231,485.7451;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;91;-731.6473,379.2185;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;53;-896.7832,16.67318;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;58;-875.2192,128.332;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;116;-467.2045,386.9525;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;117;-461.5048,470.5531;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-721.6726,682.4335;Float;False;Property;_DifferenceThreshold;Difference Threshold;4;0;Create;True;0;0;False;0;0;0;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;96;-303.6713,411.4413;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-681.7908,71.68948;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-678.827,588.9836;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-421.8257,228.5361;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;100;-145.8477,538.6387;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-616.3525,-208.0113;Inherit;True;Property;_BaseTexture;Base Texture;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;92.92843,269.5623;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;79;-189.8604,-60.81562;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;52;-169.3876,54.70139;Inherit;True;Property;_UVTexture;UV Texture;2;0;Create;True;0;0;False;0;-1;d611d6f49dd07344aa1277ada0f6bf6c;d611d6f49dd07344aa1277ada0f6bf6c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;98.50198,-126.1641;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;295.7195,105.5482;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;476.3192,-97.63742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;119;432.194,-318.6396;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;465.8525,197.6619;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;740.1755,-148.5417;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;ASESampleShaders/UV Light Reveal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;False;Transparent;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;85;0;84;0
WireConnection;85;1;54;2
WireConnection;92;0;87;0
WireConnection;91;0;85;0
WireConnection;116;0;91;0
WireConnection;117;0;92;0
WireConnection;96;0;116;0
WireConnection;96;1;117;0
WireConnection;59;0;53;0
WireConnection;59;1;58;0
WireConnection;55;0;59;0
WireConnection;55;1;54;2
WireConnection;100;0;96;0
WireConnection;100;1;98;0
WireConnection;100;3;98;0
WireConnection;100;5;90;0
WireConnection;99;0;55;0
WireConnection;99;1;100;0
WireConnection;79;0;59;0
WireConnection;80;0;51;0
WireConnection;80;1;79;0
WireConnection;57;0;52;1
WireConnection;57;1;99;0
WireConnection;78;0;80;0
WireConnection;78;1;57;0
WireConnection;118;0;78;0
WireConnection;118;1;52;1
WireConnection;0;2;119;0
WireConnection;0;10;118;0
ASEEND*/
//CHKSM=EEBA3E339AB42B8EF0EF33AAC5DD1A6EE43C9F82