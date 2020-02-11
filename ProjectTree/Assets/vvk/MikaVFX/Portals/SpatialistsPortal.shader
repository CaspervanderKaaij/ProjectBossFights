// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpatialistsPortal"
{
	Properties
	{
		_TimeSped("TimeSped", Float) = 1
		_Scales("Scales", Vector) = (1,0,1,0)
		_FresP("FresP", Float) = 0
		[HDR]_Color1("Color 1", Color) = (0,0,0,0)
		_Scale("Scale", Vector) = (0,0,0,0)
		_Stars("Stars", 2D) = "white" {}
		_OffsetMultiplier("OffsetMultiplier", Float) = 1
		[HDR]_BepicStars("BepicStars", Color) = (1,1,1,0)
		[HDR]_Color0("Color 0", Color) = (1,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 worldNormal;
		};

		uniform float2 _Scale;
		uniform float _TimeSped;
		uniform float _OffsetMultiplier;
		uniform float3 _Scales;
		uniform float4 _BepicStars;
		uniform sampler2D _Stars;
		uniform sampler2D _Sampler050;
		uniform float4 _Stars_ST;
		uniform float _FresP;
		uniform float4 _Color0;
		uniform float4 _Color1;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime24 = _Time.y * _TimeSped;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( ase_vertex3Pos + ( float3( sin( ( _Scale * ( mulTime24 + ase_worldPos.y ) ) ) ,  0.0 ) * ase_vertexNormal * _OffsetMultiplier ) ) * _Scales );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV40 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode40 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV40, _FresP ) );
			float4 lerpResult43 = lerp( _Color0 , _Color1 , fresnelNode40);
			o.Emission = ( ( _BepicStars * ( tex2D( _Stars, (ase_screenPosNorm*float4( _Stars_ST.xy, 0.0 , 0.0 ) + float4( _Stars_ST.zw, 0.0 , 0.0 )).xy ) * ( 1.0 - fresnelNode40 ) ) ) + lerpResult43 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
993;73;648;651;1281.067;-116.2141;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;31;-1067.054,191.2372;Inherit;False;Property;_TimeSped;TimeSped;0;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;24;-925.8214,193.4763;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;32;-1041.773,366.0125;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureTransformNode;50;-827.3875,-468.4998;Inherit;False;45;False;1;0;SAMPLER2D;_Sampler050;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.ScreenPosInputsNode;44;-867.0818,-716.5179;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;34;-768.6329,-3.630196;Inherit;False;Property;_Scale;Scale;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;41;-220.338,-13.86765;Inherit;False;Property;_FresP;FresP;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-716.4,191.2117;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;49;-658.2972,-592.2604;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-588.2741,77.52556;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;40;-78.23103,-26.68929;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-451.7819,-717.9181;Inherit;True;Property;_Stars;Stars;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-543.0539,440.2372;Inherit;False;Property;_OffsetMultiplier;OffsetMultiplier;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-33.26361,-517.644;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;26;-391.3032,505.9495;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;23;-542.8214,195.4763;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;27;-307.3032,140.9495;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-212.956,-482.3386;Inherit;False;Property;_Color0;Color 0;8;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-158.6223,-630.3932;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;42;-181.8728,-259.6165;Inherit;False;Property;_Color1;Color 1;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;-213.9714,-903.7987;Inherit;False;Property;_BepicStars;BepicStars;7;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-296.3032,395.9495;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;43;44.64314,-342.9573;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-33.97144,-697.7987;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-121.3032,307.9495;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;38;-95.61523,456.4108;Inherit;False;Property;_Scales;Scales;1;0;Create;True;0;0;False;0;1,0,1;1,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;107.3848,394.4108;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;205.0844,-417.6655;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-698.3425,385.8222;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;58;-933.0669,532.2141;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-832.0669,505.2141;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;236,-31;Float;False;True;-1;6;ASEMaterialInspector;0;0;Unlit;SpatialistsPortal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;21.3;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;31;0
WireConnection;39;0;24;0
WireConnection;39;1;32;2
WireConnection;49;0;44;0
WireConnection;49;1;50;0
WireConnection;49;2;50;1
WireConnection;35;0;34;0
WireConnection;35;1;39;0
WireConnection;40;3;41;0
WireConnection;45;1;49;0
WireConnection;53;0;40;0
WireConnection;23;0;35;0
WireConnection;52;0;45;0
WireConnection;52;1;53;0
WireConnection;29;0;23;0
WireConnection;29;1;26;0
WireConnection;29;2;30;0
WireConnection;43;0;22;0
WireConnection;43;1;42;0
WireConnection;43;2;40;0
WireConnection;47;0;46;0
WireConnection;47;1;52;0
WireConnection;28;0;27;0
WireConnection;28;1;29;0
WireConnection;37;0;28;0
WireConnection;37;1;38;0
WireConnection;48;0;47;0
WireConnection;48;1;43;0
WireConnection;59;1;32;2
WireConnection;0;2;48;0
WireConnection;0;11;37;0
ASEEND*/
//CHKSM=CF12CB831E6CCA36D461ABB66C902FF3EA78850E