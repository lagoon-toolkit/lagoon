You can drop SVG files containing several images per file as :

<?xml version="1.0" encoding="utf-8"?>
<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
	<symbol id="xxxxxxx" class="bi bi-xxxxxxx" viewBox="0 0 16 16" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
		<path d="..." />
		<path d="..." />
	</symbol>
	<symbol id="zzzzzzz" class="bi bi-zzzzzzz" viewBox="0 0 24 24" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
		<path d="..." />
		<path d="..." />
	</symbol>
	...
</svg>

WARNING : all above symbol attributes are required !