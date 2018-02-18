// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Custom/FogOfWar" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FogRadius("Fog Radius", Float) = 1.0
		_FogMaxRadius("Fog Max Radius", Float) = 1.0
		_Player1Pos("Player1 Position", Vector) = (0, 0, 0, 1)
		_Player2Pos("Player2 Position", Vector) = (0, 0, 0, 1)
		_Player3Pos("Player3 Position", Vector) = (0, 0, 0, 1)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha 
		ZWrite Off
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alpha:fade

		struct Input 
		{
			float2 uv_MainTex;
			float2 location;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		float _FogRadius;
		float _FogMaxRadius;
		float4 _Player1Pos;
		float4 _Player2Pos;
		float4 _Player3Pos;

		float PowerForPosition(float4 pos, float2 nearVertex)
		{
			float attenuation = clamp(_FogRadius - length(pos.xz - nearVertex), 0.0f, _FogRadius);
			return (1.0 / _FogMaxRadius) * (attenuation / _FogRadius);
		}

		void vert(inout appdata_full vertexData, out Input outData)
		{
			float4 pos = UnityObjectToClipPos(vertexData.vertex);
			float4 worldPos = mul(unity_ObjectToWorld, vertexData.vertex);
			outData.uv_MainTex = vertexData.texcoord;
			outData.location = worldPos.xz;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 baseColor = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = baseColor.rgb;

			float alpha = 1.0 - (baseColor.a +
				PowerForPosition(_Player1Pos, IN.location) + 
				PowerForPosition(_Player2Pos, IN.location) + 
				PowerForPosition(_Player3Pos, IN.location));
			o.Alpha = alpha;
		}

		ENDCG
	}
}
