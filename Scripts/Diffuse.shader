Shader "KSP/LightWrap/Diffuse"
{
	Properties 
	{
		_MainTex("_MainTex (RGB)", 2D) = "white" {}
		_Color ("LightWrap", Color) = (0,0,0,0)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha 

		CGPROGRAM



		//#pragma surface surf Lambert
		#pragma target 3.0
		
		#pragma surface surf WrapLambert

    	half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
        half NdotL = dot (s.Normal, lightDir);
        //half diff = NdotL * 0.5 + 0.5;
		float3 w = _Color.rgb*0.5;;
        half diff = NdotL * 0.5 + w;
        half4 c;
        c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
        c.a = s.Alpha;
        return c;
		}
		
		struct Input
		{
			float2 uv_MainTex;

		};

		sampler2D _MainTex;
		float4 _Color;
		float3 w;

		void surf (Input IN, inout SurfaceOutput o)
		{
			//float4 color = tex2D(_MainTex,(IN.uv_MainTex));
        	o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;


			//o.Albedo = color.rgb;
			o.Gloss = 0;
			o.Specular = 0;
		}
		ENDCG
	}
	Fallback "Diffuse"
}