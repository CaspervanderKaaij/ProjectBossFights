// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rift"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_DistortionMultiplier("DistortionMultiplier", Float) = 1
		[HDR]_EdgeLight("EdgeLight", Color) = (1,1,1,0)
		_Second("Second", Float) = 0.99
		_First("First", Float) = 0.96
		_NoiseSpeed("NoiseSpeed", Float) = 1
		_NoiseScale("NoiseScale", Float) = 6
		_NoiseStrength("NoiseStrength", Float) = 1
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_Casper("Casper", Range( 0 , 1)) = 0.6756577
		_V1("V1", Float) = 4
		_V2("V2", Float) = 10
		_OffsetoMundoScalo("OffsetoMundoScalo", Float) = 7.14
		_OffsetoMundoStrength("OffsetoMundoStrength", Float) = 0.1
		_Roeko1("Roeko1", Vector) = (0,0,0,0)
		_Roeko2("Roeko2", Vector) = (0.1,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _EdgeLight;
		uniform sampler2D _TextureSample0;
		uniform float2 _Roeko1;
		uniform float _OffsetoMundoScalo;
		uniform float2 _Roeko2;
		uniform float _OffsetoMundoStrength;
		uniform float _Second;
		uniform float _First;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortionMultiplier;
		uniform float _NoiseScale;
		uniform float _NoiseSpeed;
		uniform float _NoiseStrength;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _TextureSample2;
		uniform sampler2D _Sampler055;
		uniform float4 _TextureSample2_ST;
		uniform float _V2;
		uniform float _V1;
		uniform float _Casper;


		//https://www.shadertoy.com/view/XdXGW8
		float2 GradientNoiseDir( float2 x )
		{
			const float2 k = float2( 0.3183099, 0.3678794 );
			x = x * k + k.yx;
			return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
		}
		
		float GradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 i = floor( p );
			float2 f = frac( p );
			float2 u = f * f * ( 3.0 - 2.0 * f );
			return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
					lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
		}


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


		float2 voronoihash38( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi38( float2 v, float time, inout float2 id )
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
			 		float2 o = voronoihash38( n + g );
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


		float2 voronoihash49( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi49( float2 v, float time, inout float2 id )
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
			 		float2 o = voronoihash49( n + g );
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
			return F2 - F1;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord1 = i.uv_texcoord * float2( 1.5,1.5 ) + float2( -0.14,-0.27 );
			float cos29 = cos( 0.81 );
			float sin29 = sin( 0.81 );
			float2 rotator29 = mul( uv_TexCoord1 - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
			float2 panner64 = ( 1.0 * _Time.y * _Roeko1 + rotator29);
			float gradientNoise57 = GradientNoise(panner64,_OffsetoMundoScalo);
			gradientNoise57 = gradientNoise57*0.5 + 0.5;
			float2 panner66 = ( 1.0 * _Time.y * _Roeko2 + rotator29);
			float gradientNoise63 = GradientNoise(panner66,_OffsetoMundoScalo);
			gradientNoise63 = gradientNoise63*0.5 + 0.5;
			float4 tex2DNode2 = tex2D( _TextureSample0, ( rotator29 + ( (-1.0 + (( gradientNoise57 * gradientNoise63 ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _OffsetoMundoStrength ) ), float2( 0,0 ), float2( 0,0 ) );
			float temp_output_25_0 = step( tex2DNode2.r , _First );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float mulTime41 = _Time.y * _NoiseSpeed;
			float time38 = mulTime41;
			float2 coords38 = rotator29 * _NoiseScale;
			float2 id38 = 0;
			float fade38 = 0.5;
			float voroi38 = 0;
			float rest38 = 0;
			for( int it = 0; it <8; it++ ){
			voroi38 += fade38 * voronoi38( coords38, time38, id38 );
			rest38 += fade38;
			coords38 *= 2;
			fade38 *= 0.5;
			}
			voroi38 /= rest38;
			float4 lerpResult7 = lerp( ase_grabScreenPosNorm , ( _DistortionMultiplier * ( voroi38 * _NoiseStrength * ( tex2DNode2.r + ase_grabScreenPosNorm ) ) ) , tex2DNode2.r);
			float4 screenColor9 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,lerpResult7.xy/lerpResult7.w);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float time49 = _V1;
			float2 coords49 = ase_screenPosNorm.xy * _V2;
			float2 id49 = 0;
			float voroi49 = voronoi49( coords49, time49,id49 );
			float4 lerpResult48 = lerp( tex2D( _TextureSample1, ase_screenPosNorm.xy ) , tex2D( _TextureSample2, (ase_screenPosNorm*float4( _TextureSample2_ST.xy, 0.0 , 0.0 ) + float4( _TextureSample2_ST.zw, 0.0 , 0.0 )).xy ) , step( voroi49 , _Casper ));
			o.Emission = ( ( _EdgeLight * ( step( tex2DNode2.r , _Second ) - temp_output_25_0 ) ) + ( ( screenColor9 * ( 1.0 - tex2DNode2.r ) ) + ( ( 1.0 - temp_output_25_0 ) * lerpResult48 ) ) ).rgb;
			float smoothstepResult27 = smoothstep( 0.0 , 0.15 , tex2DNode2.r);
			o.Alpha = smoothstepResult27;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
0;73;1924;928;5044.63;-224.331;2.579544;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2176.64,52.54858;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.5,1.5;False;1;FLOAT2;-0.14,-0.27;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-2145.714,254.1017;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;0.81;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;29;-1927.714,158.1016;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;68;-2884.431,679.7991;Inherit;False;Property;_Roeko2;Roeko2;17;0;Create;True;0;0;False;0;0.1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;67;-2880.431,511.7991;Inherit;False;Property;_Roeko1;Roeko1;16;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;58;-2632.157,436.113;Inherit;False;Property;_OffsetoMundoScalo;OffsetoMundoScalo;14;0;Create;True;0;0;False;0;7.14;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;64;-2397.272,514.1974;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;66;-2617.39,753.1577;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;57;-2180.558,501.5128;Inherit;True;Gradient;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;63;-2398.025,771.473;Inherit;True;Gradient;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2091.431,787.7991;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1933.21,710.7332;Inherit;False;Property;_OffsetoMundoStrength;OffsetoMundoStrength;15;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;56;-1589.458,424.2129;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1564.21,678.7332;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1881.063,430.975;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2317.469,-400.7237;Inherit;False;Property;_NoiseSpeed;NoiseSpeed;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2001.47,-265.7237;Inherit;False;Property;_NoiseScale;NoiseScale;8;0;Create;True;0;0;False;0;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1232.54,81.3486;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;f35cef3a157cf9844a9606139f3998fb;279697e2758c5724aad60d6dc5a0523a;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;41;-2097.469,-422.7238;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;5;-1211.566,-198.7779;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-919.783,-221.7693;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VoronoiNode;38;-1780.911,-437.3021;Inherit;True;0;0;1;1;8;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;4.24;False;2;FLOAT;6.3;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.RangedFloatNode;43;-1580.632,-172.4707;Inherit;False;Property;_NoiseStrength;NoiseStrength;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-165.1754,1290.822;Inherit;False;Property;_V2;V2;13;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;12;-861.2873,1291.833;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureTransformNode;55;-1008.481,1561.576;Inherit;False;47;False;1;0;SAMPLER2D;_Sampler055;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1239.669,-369.6167;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-931.0947,-459.8441;Inherit;False;Property;_DistortionMultiplier;DistortionMultiplier;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-196.3754,1046.722;Inherit;False;Property;_V1;V1;12;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;99.47192,1555.746;Inherit;False;Property;_Casper;Casper;11;0;Create;True;0;0;False;0;0.6756577;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1237.894,530.5276;Inherit;False;Property;_First;First;5;0;Create;True;0;0;False;0;0.96;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;8;-633.9902,-631.319;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;54;-713.4814,1581.677;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-680.0947,-330.8441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VoronoiNode;49;-26.5411,1254.371;Inherit;False;0;0;1;2;1;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;4.03;False;2;FLOAT;10.49;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.StepOpNode;50;216.0712,1308.886;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;25;-968.7336,620.6208;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.96;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-911.494,469.6279;Inherit;False;Property;_Second;Second;4;0;Create;True;0;0;False;0;0.99;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;7;-422.4087,-247.3065;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;11;-504.087,1182.233;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;-1;3a712e4b235b5e5418a6e9162df96fa5;3a712e4b235b5e5418a6e9162df96fa5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;47;-497.2275,1479.099;Inherit;True;Property;_TextureSample2;Texture Sample 2;10;0;Create;True;0;0;False;0;-1;92a729647ea87944b89cf5a581551466;92a729647ea87944b89cf5a581551466;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;31;-732.5872,347.837;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-403.8661,905.2603;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;22;-516.7054,191.656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-137.1584,1482.004;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;9;-141.7654,-161.7096;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;11.87066,996.9979;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-432.7878,469.3369;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-7.5463,150.823;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;34;-643.8218,-38.60355;Inherit;False;Property;_EdgeLight;EdgeLight;3;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;21;266.9373,330.6849;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-301.3217,42.39644;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1344.212,-210.2875;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;277.3969,-171.5922;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;27;-140.1851,370.8049;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;601.8999,-11.6;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Rift;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;6;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;1;0
WireConnection;29;2;30;0
WireConnection;64;0;29;0
WireConnection;64;2;67;0
WireConnection;66;0;29;0
WireConnection;66;2;68;0
WireConnection;57;0;64;0
WireConnection;57;1;58;0
WireConnection;63;0;66;0
WireConnection;63;1;58;0
WireConnection;69;0;57;0
WireConnection;69;1;63;0
WireConnection;56;0;69;0
WireConnection;61;0;56;0
WireConnection;61;1;62;0
WireConnection;60;0;29;0
WireConnection;60;1;61;0
WireConnection;2;1;60;0
WireConnection;41;0;40;0
WireConnection;6;0;2;1
WireConnection;6;1;5;0
WireConnection;38;0;29;0
WireConnection;38;1;41;0
WireConnection;38;2;42;0
WireConnection;44;0;38;0
WireConnection;44;1;43;0
WireConnection;44;2;6;0
WireConnection;54;0;12;0
WireConnection;54;1;55;0
WireConnection;54;2;55;1
WireConnection;18;0;19;0
WireConnection;18;1;44;0
WireConnection;49;0;12;0
WireConnection;49;1;52;0
WireConnection;49;2;53;0
WireConnection;50;0;49;0
WireConnection;50;1;51;0
WireConnection;25;0;2;1
WireConnection;25;1;36;0
WireConnection;7;0;8;0
WireConnection;7;1;18;0
WireConnection;7;2;2;1
WireConnection;11;1;12;0
WireConnection;47;1;54;0
WireConnection;31;0;2;1
WireConnection;31;1;37;0
WireConnection;26;0;25;0
WireConnection;22;0;2;1
WireConnection;48;0;11;0
WireConnection;48;1;47;0
WireConnection;48;2;50;0
WireConnection;9;0;7;0
WireConnection;13;0;26;0
WireConnection;13;1;48;0
WireConnection;32;0;31;0
WireConnection;32;1;25;0
WireConnection;20;0;9;0
WireConnection;20;1;22;0
WireConnection;21;0;20;0
WireConnection;21;1;13;0
WireConnection;33;0;34;0
WireConnection;33;1;32;0
WireConnection;35;0;33;0
WireConnection;35;1;21;0
WireConnection;27;0;2;1
WireConnection;0;2;35;0
WireConnection;0;9;27;0
ASEEND*/
//CHKSM=533F76143F158A7B2423C208C0C52FCAA75D8659