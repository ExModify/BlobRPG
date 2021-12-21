#version 330

out vec4 outColor;

uniform sampler2D modelTexture;
in vec2 textureCoords;

void main(void){
	
	float alpha = texture(modelTexture, textureCoords).a;
	if (alpha < 0.5) {
		discard;
	}

	outColor = vec4(1.0);
	
}