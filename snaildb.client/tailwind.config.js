/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
      extend: {},
  },
  safelist: [
      { pattern: /^bg-(green|blue|purple|orange)-500\/\d{1,3}$/, variants: ['hover'] }
  ],
  plugins: [],
}
