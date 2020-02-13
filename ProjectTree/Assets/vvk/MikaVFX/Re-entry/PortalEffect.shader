// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PortalEffect"
{
	Properties
	{
		_Noise("Noise", 2D) = "white" {}
		_Skiddadle("Skiddadle", Float) = -0.1
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		_Panning("Panning", Vector) = (0,0,0,0)
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
		};

		uniform float4 _Color0;
		uniform sampler2D _Noise;
		uniform float2 _Panning;
		uniform float4 _Noise_ST;
		uniform float _Skiddadle;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color0.rgb;
			float2 uv0_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * _Panning + uv0_Noise);
			float smoothstepResult14 = smoothstep( tex2D( _Noise, panner9 ).r , 1.0 , ( 1.0 - ( uv0_Noise.y - _Skiddadle ) ));
			o.Alpha = smoothstepResult14;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
911;73;594;651;1460.681;69.84865;1.6;True;False
Node;AmplifyShaderEditor.Vector2Node;7;-1233.349,420.9182;Inherit;False;Property;_Panning;Panning;3;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;8;-535.3521,193.2126;Inherit;False;Property;_Skiddadle;Skiddadle;1;0;Create;True;0;0;False;0;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1297.651,234.9703;Inherit;False;0;12;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;9;-1006.463,356.0934;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-513.3521,293.2126;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-333.3521,318.2126;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-771.3521,426.2126;Inherit;True;Property;_Noise;Noise;0;0;Create;True;0;0;False;0;-1;c60ce9d4c710434479c26cd5a98f7a9c;c60ce9d4c710434479c26cd5a98f7a9c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-347.4297,-107.2276;Inherit;False;Property;_Color0;Color 0;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;14;-356.152,437.4126;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;PortalEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;6;0
WireConnection;9;2;7;0
WireConnection;10;0;6;2
WireConnection;10;1;8;0
WireConnection;11;0;10;0
WireConnection;12;1;9;0
WireConnection;14;0;11;0
WireConnection;14;1;12;0
WireConnection;0;2;5;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=4C6D1BADA179D50086F3DDFA71D1A705CFB91A04