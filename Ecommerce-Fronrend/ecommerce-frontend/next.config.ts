import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Enable React strict mode for better development experience
  reactStrictMode: true,
  
  // Configure image domains if you're using external images
  images: {
    domains: ['your-image-domain.com', 'cdn.example.com'],
    // Or use remotePatterns for better security (Next.js 13+)
    remotePatterns: [
      {
        protocol: 'https',
        hostname: '**',
      },
    ],
  },
  
  // Environment variables that will be exposed to the browser
  env: {
    CUSTOM_KEY: process.env.CUSTOM_KEY,
  },
  
  // API rewrites to avoid CORS issues in development
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5000/api/:path*', // Your backend URL
      },
    ];
  },
  
  // Output configuration for deployment
  output: 'standalone', // For Docker deployments
  
  // Compiler options
  compiler: {
    removeConsole: process.env.NODE_ENV === 'production',
  },
};

export default nextConfig;