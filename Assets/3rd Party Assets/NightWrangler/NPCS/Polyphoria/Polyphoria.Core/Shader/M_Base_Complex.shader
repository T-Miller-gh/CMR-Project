// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Base_Complex"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_MSO("MSO", 2D) = "white" {}
		_TintMask("Tint Mask", 2D) = "white" {}
		_RSmoothnessMultiplier("R: Smoothness Multiplier", Float) = 1
		_GSmoothnessMultiplier("G: Smoothness Multiplier", Float) = 1
		_BSmoothnessMultiplier("B: Smoothness Multiplier", Float) = 1
		_ASmoothnessMultiplier("A: Smoothness Multiplier", Float) = 1
		_YSmoothnessMultiplier("Y: Smoothness Multiplier", Float) = 1
		_CSmoothnessMultiplier("C: Smoothness Multiplier", Float) = 1
		_MSmoothnessMultiplier("M: Smoothness Multiplier", Float) = 1
		[HDR]_RPrimary("R: Primary", Color) = (1,0,0,0)
		[HDR]_GDetails1("G: Details 1", Color) = (0,1,0,0)
		[HDR]_BDetails2("B: Details 2", Color) = (0,0,1,0)
		[HDR]_C("C", Color) = (0,1,1,0)
		[HDR]_M("M", Color) = (1,0,1,0)
		[HDR]_Y("Y", Color) = (1,1,0,0)
		[HDR]_ADetails3Skin("A: Details 3 / Skin", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _TintMask;
		uniform float4 _TintMask_ST;
		uniform float4 _RPrimary;
		uniform float4 _GDetails1;
		uniform float4 _BDetails2;
		uniform float4 _Y;
		uniform float4 _M;
		uniform float4 _C;
		uniform float4 _ADetails3Skin;
		uniform sampler2D _MSO;
		uniform float4 _MSO_ST;
		uniform float _RSmoothnessMultiplier;
		uniform float _GSmoothnessMultiplier;
		uniform float _BSmoothnessMultiplier;
		uniform float _YSmoothnessMultiplier;
		uniform float _MSmoothnessMultiplier;
		uniform float _CSmoothnessMultiplier;
		uniform float _ASmoothnessMultiplier;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_54_0_g14 = tex2DNode1;
			float3 desaturateInitialColor115_g14 = temp_output_54_0_g14.xyz;
			float desaturateDot115_g14 = dot( desaturateInitialColor115_g14, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar115_g14 = lerp( desaturateInitialColor115_g14, desaturateDot115_g14.xxx, 1.0 );
			float2 uv_TintMask = i.uv_texcoord * _TintMask_ST.xy + _TintMask_ST.zw;
			float4 tex2DNode3 = tex2D( _TintMask, uv_TintMask );
			float4 break41_g14 = tex2DNode3;
			float temp_output_64_0_g14 = ( break41_g14.r * break41_g14.g );
			float temp_output_81_0_g14 = ( break41_g14.r * break41_g14.b );
			float clampResult65_g14 = clamp( ( temp_output_64_0_g14 + temp_output_81_0_g14 ) , 0.0 , 1.0 );
			float temp_output_42_0_g14 = ( break41_g14.g * break41_g14.b );
			float clampResult77_g14 = clamp( ( clampResult65_g14 + temp_output_42_0_g14 ) , 0.0 , 1.0 );
			float clampResult71_g14 = clamp( ( break41_g14.r - clampResult77_g14 ) , 0.0 , 1.0 );
			float4 temp_output_79_0_g14 = _RPrimary;
			float temp_output_84_0_g14 = ( clampResult71_g14 * temp_output_79_0_g14.a );
			float clampResult74_g14 = clamp( ( break41_g14.g - clampResult77_g14 ) , 0.0 , 1.0 );
			float4 temp_output_70_0_g14 = _GDetails1;
			float temp_output_88_0_g14 = ( clampResult74_g14 * temp_output_70_0_g14.a );
			float clampResult53_g14 = clamp( ( break41_g14.b - clampResult77_g14 ) , 0.0 , 1.0 );
			float4 temp_output_69_0_g14 = _BDetails2;
			float temp_output_91_0_g14 = ( clampResult53_g14 * temp_output_69_0_g14.a );
			float4 temp_output_78_0_g14 = _Y;
			float temp_output_94_0_g14 = ( temp_output_64_0_g14 * temp_output_78_0_g14.a );
			float4 temp_output_63_0_g14 = _M;
			float temp_output_97_0_g14 = ( temp_output_81_0_g14 * temp_output_63_0_g14.a );
			float4 temp_output_56_0_g14 = _C;
			float temp_output_100_0_g14 = ( temp_output_42_0_g14 * temp_output_56_0_g14.a );
			float4 temp_output_59_0_g14 = _ADetails3Skin;
			float temp_output_103_0_g14 = ( break41_g14.a * temp_output_59_0_g14.a );
			float clampResult48_g14 = clamp( ( ( ( ( ( ( temp_output_84_0_g14 + temp_output_88_0_g14 ) + temp_output_91_0_g14 ) + temp_output_94_0_g14 ) + temp_output_97_0_g14 ) + temp_output_100_0_g14 ) + temp_output_103_0_g14 ) , 0.0 , 1.0 );
			float4 lerpResult61_g14 = lerp( temp_output_54_0_g14 , ( float4( desaturateVar115_g14 , 0.0 ) * ( ( ( ( ( ( ( temp_output_84_0_g14 * temp_output_79_0_g14 ) + ( temp_output_88_0_g14 * temp_output_70_0_g14 ) ) + ( temp_output_91_0_g14 * temp_output_69_0_g14 ) ) + ( temp_output_94_0_g14 * temp_output_78_0_g14 ) ) + ( temp_output_97_0_g14 * temp_output_63_0_g14 ) ) + ( temp_output_100_0_g14 * temp_output_56_0_g14 ) ) + ( temp_output_103_0_g14 * temp_output_59_0_g14 ) ) ) , clampResult48_g14);
			o.Albedo = lerpResult61_g14.rgb;
			float2 uv_MSO = i.uv_texcoord * _MSO_ST.xy + _MSO_ST.zw;
			float4 tex2DNode4 = tex2D( _MSO, uv_MSO );
			o.Metallic = tex2DNode4.r;
			float4 temp_cast_5 = (tex2DNode4.g).xxxx;
			float4 temp_output_21_0_g15 = temp_cast_5;
			float4 break26_g15 = tex2DNode3;
			float temp_output_47_0_g15 = ( break26_g15.r * break26_g15.g );
			float temp_output_48_0_g15 = ( break26_g15.r * break26_g15.b );
			float clampResult55_g15 = clamp( ( temp_output_47_0_g15 + temp_output_48_0_g15 ) , 0.0 , 1.0 );
			float temp_output_49_0_g15 = ( break26_g15.g * break26_g15.b );
			float clampResult56_g15 = clamp( ( clampResult55_g15 + temp_output_49_0_g15 ) , 0.0 , 1.0 );
			float clampResult57_g15 = clamp( ( break26_g15.r - clampResult56_g15 ) , 0.0 , 1.0 );
			float clampResult58_g15 = clamp( ( break26_g15.g - clampResult56_g15 ) , 0.0 , 1.0 );
			float clampResult59_g15 = clamp( ( break26_g15.b - clampResult56_g15 ) , 0.0 , 1.0 );
			float clampResult75_g15 = clamp( ( ( ( break26_g15.r + break26_g15.g ) + break26_g15.b ) + break26_g15.a ) , 0.0 , 1.0 );
			float4 lerpResult24_g15 = lerp( temp_output_21_0_g15 , ( temp_output_21_0_g15 * ( ( ( ( ( ( ( clampResult57_g15 * _RSmoothnessMultiplier ) + ( clampResult58_g15 * _GSmoothnessMultiplier ) ) + ( clampResult59_g15 * _BSmoothnessMultiplier ) ) + ( temp_output_47_0_g15 * _YSmoothnessMultiplier ) ) + ( temp_output_48_0_g15 * _MSmoothnessMultiplier ) ) + ( temp_output_49_0_g15 * _CSmoothnessMultiplier ) ) + ( break26_g15.a * _ASmoothnessMultiplier ) ) ) , clampResult75_g15);
			o.Smoothness = lerpResult24_g15.x;
			o.Occlusion = tex2DNode4.b;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
-1765;157;1275;964;804.7968;1049.582;1.191029;True;True
Node;AmplifyShaderEditor.RangedFloatNode;12;-211.067,521.5726;Float;False;Property;_GSmoothnessMultiplier;G: Smoothness Multiplier;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-183.126,-722.2538;Float;False;Property;_ADetails3Skin;A: Details 3 / Skin;18;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;99.85449,-894.1841;Float;False;Property;_C;C;15;1;[HDR];Create;True;0;0;False;0;0,1,1,0;0,1,1,0.5019608;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;29;-179.2177,-887.9811;Float;False;Property;_BDetails2;B: Details 2;14;1;[HDR];Create;True;0;0;False;0;0,0,1,0;1,0.2249288,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;36;333.6718,-850.7115;Float;False;Property;_M;M;16;1;[HDR];Create;True;0;0;False;0;1,0,1,0;1,0,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-402.3617,-883.3957;Float;False;Property;_RPrimary;R: Primary;12;1;[HDR];Create;True;0;0;False;0;1,0,0,0;0.4245283,0.4658239,0.5660378,0.8039216;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;37;120.6576,-715.9474;Float;False;Property;_Y;Y;17;1;[HDR];Create;True;0;0;False;0;1,1,0,0;1,1,0,0.5176471;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-256,-208;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;588366a03a58c694d964819fb48c27ca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;28;-399.3784,-716.2192;Float;False;Property;_GDetails1;G: Details 1;13;1;[HDR];Create;True;0;0;False;0;0,1,0,0;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-220.8706,391.4289;Float;False;Property;_RSmoothnessMultiplier;R: Smoothness Multiplier;5;0;Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-252.2446,184.8285;Float;True;Property;_MSO;MSO;3;0;Create;True;0;0;False;0;None;7c80e3518019e25458ae38ed3e16f964;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-236.9483,923.0995;Float;False;Property;_MSmoothnessMultiplier;M: Smoothness Multiplier;11;0;Create;True;0;0;False;0;1;5.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-208.3531,592.6926;Float;False;Property;_BSmoothnessMultiplier;B: Smoothness Multiplier;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-209.6483,826.8999;Float;False;Property;_CSmoothnessMultiplier;C: Smoothness Multiplier;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-207.0483,748.8997;Float;False;Property;_YSmoothnessMultiplier;Y: Smoothness Multiplier;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-205.8522,666.2299;Float;False;Property;_ASmoothnessMultiplier;A: Smoothness Multiplier;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-256,-1;Float;True;Property;_TintMask;Tint Mask;4;0;Create;True;0;0;False;0;None;5f4d4333f5b3ec54fb10382bb251b8c0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;366.3867,313.7528;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;a85b5d6573f1a1e4291f4ed8352aef87;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;34;384.6198,-530.257;Float;False;RGBACMY_Tint;-1;;14;b5ffb0fecc81b494cab296efec87a515;0;9;56;COLOR;0,1,1,0;False;78;COLOR;1,1,0,0;False;63;COLOR;1,0,1,0;False;54;FLOAT4;0,0,0,1;False;52;COLOR;0,0,0,0;False;79;COLOR;1,0,0,0;False;70;COLOR;0,1,0,0;False;69;COLOR;0,0,1,0;False;59;COLOR;0,0,0,0;False;1;COLOR;83
Node;AmplifyShaderEditor.FunctionNode;38;270.7512,79.25607;Float;False;RGBACMY_Multiplier;-1;;15;b737af1a290888a43afdc27d71e5f905;0;9;41;FLOAT;0;False;42;FLOAT;0;False;43;FLOAT;0;False;21;FLOAT4;0,0,0,1;False;1;COLOR;0,0,0,0;False;27;FLOAT;0;False;28;FLOAT;0;False;29;FLOAT;0;False;30;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;779.8552,-199.0936;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;M_Base_Complex;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;34;56;35;0
WireConnection;34;78;37;0
WireConnection;34;63;36;0
WireConnection;34;54;1;0
WireConnection;34;52;3;0
WireConnection;34;79;27;0
WireConnection;34;70;28;0
WireConnection;34;69;29;0
WireConnection;34;59;30;0
WireConnection;38;41;40;0
WireConnection;38;42;39;0
WireConnection;38;43;41;0
WireConnection;38;21;4;2
WireConnection;38;1;3;0
WireConnection;38;27;11;0
WireConnection;38;28;12;0
WireConnection;38;29;13;0
WireConnection;38;30;14;0
WireConnection;0;0;34;83
WireConnection;0;1;15;0
WireConnection;0;3;4;1
WireConnection;0;4;38;0
WireConnection;0;5;4;3
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=595A162CBF88B24131D90E5303511D273000C85B