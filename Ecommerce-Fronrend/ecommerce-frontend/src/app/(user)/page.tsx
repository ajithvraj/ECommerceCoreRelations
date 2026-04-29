"use client";

import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import ProductCard from "@/components/ProductCard";
import api from "@/lib/axios";

interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  category: string;
  primaryImageUrl: string;
  stock: number;
  isActive: boolean;
}

export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await api.get("/Product/all");
        setProducts(response.data.data);
      } catch (error) {
        console.error("Failed to fetch products", error);
      } finally {
        setLoading(false);
      }
    };
    fetchProducts();
  }, []);

  return (
    <div className="min-h-screen bg-black text-white">

      {/* hero */}
      <section className="pt-32 pb-16 px-6 text-center">
        <motion.p
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
          className="text-xs text-zinc-500 uppercase tracking-widest mb-4"
        >
          Premium Collection
        </motion.p>
        <motion.h1
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.1 }}
          className="text-5xl md:text-7xl font-semibold tracking-tight mb-6 bg-gradient-to-b from-white to-zinc-400 bg-clip-text text-transparent"
        >
          Discover Something
          <br />
          Extraordinary
        </motion.h1>
        <motion.p
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.2 }}
          className="text-zinc-400 text-lg max-w-xl mx-auto mb-10"
        >
          Curated products designed for those who appreciate quality and craftsmanship.
        </motion.p>
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.3 }}
          className="flex gap-4 justify-center"
        >
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            className="bg-white text-black px-8 py-3 rounded-full text-sm font-medium hover:bg-zinc-200 transition-colors"
          >
            Shop now
          </motion.button>
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            className="text-zinc-400 hover:text-white px-8 py-3 rounded-full text-sm transition-colors border border-white/10 hover:border-white/20"
          >
            Learn more →
          </motion.button>
        </motion.div>
      </section>

      {/* products */}
      <section className="max-w-7xl mx-auto px-6 pb-24">
        <div className="flex items-center justify-between mb-10">
          <div>
            <p className="text-xs text-zinc-500 uppercase tracking-widest mb-1">
              Our Collection
            </p>
            <h2 className="text-2xl font-semibold text-white">
              Featured Products
            </h2>
          </div>
          <motion.button
            whileHover={{ scale: 1.02 }}
            className="text-sm text-zinc-400 hover:text-white transition-colors"
          >
            View all →
          </motion.button>
        </div>

        {/* skeleton */}
        {loading && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {Array.from({ length: 8 }).map((_, i) => (
              <div
                key={i}
                className="bg-zinc-900 rounded-3xl overflow-hidden border border-white/5 animate-pulse"
              >
                <div className="aspect-square bg-zinc-800" />
                <div className="p-5 space-y-3">
                  <div className="h-3 bg-zinc-800 rounded-full w-1/3" />
                  <div className="h-4 bg-zinc-800 rounded-full w-3/4" />
                  <div className="h-3 bg-zinc-800 rounded-full w-full" />
                  <div className="h-3 bg-zinc-800 rounded-full w-2/3" />
                </div>
              </div>
            ))}
          </div>
        )}

        {/* products grid */}
        {!loading && products.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {products.map((product, index) => (
              <ProductCard
                key={product.id}
                product={product}
                index={index}
              />
            ))}
          </div>
        )}

        {/* empty */}
        {!loading && products.length === 0 && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="text-center py-24"
          >
            <p className="text-zinc-600 text-lg">No products available yet.</p>
            <p className="text-zinc-700 text-sm mt-2">
              Check back soon for new arrivals.
            </p>
          </motion.div>
        )}
      </section>

      {/* footer */}
      {/* <footer className="border-t border-white/5 py-10 px-6">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
          <span className="text-zinc-600 text-sm">
            © 2025 ECommerceCore. All rights reserved.
          </span>
          <div className="flex gap-6">
            {["Privacy", "Terms", "Contact"].map((item) => (
              
                key={item}
                href="#"
                className="text-sm text-zinc-600 hover:text-zinc-400 transition-colors"
              >
                {item}
              </a>
            ))}
          </div>
        </div>
      </footer> */}
    </div>
  );
}