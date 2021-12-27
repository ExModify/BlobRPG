#version 150

in vec2 textureCoords;

out vec4 out_Color;

uniform sampler2D colorTexture;

void main(void){
	vec4 color = texture(colorTexture, textureCoords);
	float brightness = (color.r * 0.2126) + (color.g * 0.7152) + (color.b * 0.0722);
	out_Color = color * brightness;
}