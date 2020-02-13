// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Freefall"
{
	Properties
	{
		_Gradient("Gradient", 2D) = "white" {}
		_Offset("Offset", Vector) = (0,0,0,0)
		_N("N", 2D) = "white" {}
		_S("S", 2D) = "white" {}
		_CloudPan("CloudPan", Vector) = (0,0,0,0)
		[HDR]_ColorReentry("ColorReentry", Color) = (0,0,0,0)
		_StarSped("StarSped", Vector) = (0,0,0,0)
		_Alpha("Alpha", Float) = 1
		_ReEntryStrength("ReEntryStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		AlphaToMask On
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _ColorReentry;
		uniform sampler2D _N;
		uniform float2 _CloudPan;
		uniform float4 _N_ST;
		uniform float _ReEntryStrength;
		uniform sampler2D _S;
		uniform float2 _StarSped;
		uniform float4 _S_ST;
		uniform sampler2D _Gradient;
		uniform sampler2D _Sampler012;
		uniform float4 _Gradient_ST;
		uniform float2 _Offset;
		uniform float _Alpha;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv0_N = i.uv_texcoord * _N_ST.xy + _N_ST.zw;
			float2 panner10 = ( 1.0 * _Time.y * _CloudPan + uv0_N);
			float2 uv0_S = i.uv_texcoord * _S_ST.xy + _S_ST.zw;
			float2 panner15 = ( 1.0 * _Time.y * _StarSped + uv0_S);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 lerpResult3 = lerp( ( _ColorReentry * tex2D( _N, panner10 ) * _ReEntryStrength ) , tex2D( _S, panner15 ) , tex2D( _Gradient, (ase_screenPosNorm*float4( _Gradient_ST.xy, 0.0 , 0.0 ) + float4( _Offset, 0.0 , 0.0 )).xy ).r);
			o.Emission = lerpResult3.rgb;
			o.Alpha = _Alpha;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
911;73;594;651;677.4829;710.1851;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1097.834,-503.2688;Inherit;False;0;8;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;11;-957.2809,-638.1047;Inherit;False;Property;_CloudPan;CloudPan;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1091.735,-306.6688;Inherit;False;0;9;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;10;-815.0811,-408.0045;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;16;-1084.767,-83.97516;Inherit;False;Property;_StarSped;StarSped;6;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;5;-1177.154,305.1736;Inherit;False;Property;_Offset;Offset;1;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScreenPosInputsNode;1;-1198.56,31.78034;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureTransformNode;12;-1274.731,202.301;Inherit;False;2;False;1;0;SAMPLER2D;_Sampler012;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.PannerNode;15;-918.8938,-154.9662;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;4;-993.1539,262.1736;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-437.9829,-296.1851;Inherit;False;Property;_ReEntryStrength;ReEntryStrength;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-626.6346,-533.7689;Inherit;True;Property;_N;N;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-354.0716,-635.4504;Inherit;False;Property;_ColorReentry;ColorReentry;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-722.5347,-221.3689;Inherit;True;Property;_S;S;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-783.1539,157.1736;Inherit;True;Property;_Gradient;Gradient;0;0;Create;True;0;0;False;0;-1;e77072350336caf4696373efd1c3d112;e77072350336caf4696373efd1c3d112;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-247.5684,-428.2321;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-220.4601,200.6128;Inherit;False;Property;_Alpha;Alpha;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-317.1241,-135.9716;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Freefall;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;0
WireConnection;10;2;11;0
WireConnection;15;0;7;0
WireConnection;15;2;16;0
WireConnection;4;0;1;0
WireConnection;4;1;12;0
WireConnection;4;2;5;0
WireConnection;8;1;10;0
WireConnection;9;1;15;0
WireConnection;2;1;4;0
WireConnection;13;0;14;0
WireConnection;13;1;8;0
WireConnection;13;2;18;0
WireConnection;3;0;13;0
WireConnection;3;1;9;0
WireConnection;3;2;2;1
WireConnection;0;2;3;0
WireConnection;0;9;17;0
ASEEND*/
//CHKSM=CC62D1ED4BF639AE5DCCFA774F2BAEAA19D8C626