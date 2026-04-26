"use client";

import { motion } from "framer-motion";

interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  category: string;
  primaryImageUrl: string;
  stock: number;
}

interface ProductCardProps {
  product: Product;
  index: number;
}

export default function ProductCard({ product, index }: ProductCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 40 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: index * 0.08 }}
      whileHover={{ scale: 1.02, y: -4 }}
      className="group bg-zinc-900 rounded-3xl overflow-hidden border border-white/5 hover:border-white/10 transition-colors cursor-pointer"
    >
      {/* image */}
      <div className="relative aspect-square bg-zinc-800 overflow-hidden">
        {product.primaryImageUrl ? (
          <img
            src={product.primaryImageUrl}
            alt={product.name}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-zinc-600">
            <span className="text-4xl">📦</span>
          </div>
        )}
        {/* category badge */}
        <div className="absolute top-3 left-3">
          <span className="text-xs bg-black/50 backdrop-blur-sm text-white/70 px-2.5 py-1 rounded-full border border-white/10">
            {product.category}
          </span>
        </div>
        {/* low stock badge */}
        {product.stock < 5 && product.stock > 0 && (
          <div className="absolute top-3 right-3">
            <span className="text-xs bg-red-500/80 backdrop-blur-sm text-white px-2.5 py-1 rounded-full">
              Only {product.stock} left
            </span>
          </div>
        )}
      </div>

      {/* details */}
      <div className="p-5">
        <p className="text-xs text-zinc-500 uppercase tracking-widest mb-1">
          {product.category}
        </p>
        <h3 className="text-white font-medium text-base mb-1 line-clamp-1">
          {product.name}
        </h3>
        <p className="text-zinc-500 text-sm mb-4 line-clamp-2">
          {product.description}
        </p>
        <div className="flex items-center justify-between">
          <span className="text-white font-semibold">
            ₹{product.price.toLocaleString("en-IN")}
          </span>
          <div className="flex gap-2">
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              className="text-xs bg-white/10 hover:bg-white/20 text-white px-3 py-1.5 rounded-full transition-colors"
            >
              Add to cart
            </motion.button>
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              className="text-xs bg-white text-black px-3 py-1.5 rounded-full hover:bg-zinc-200 transition-colors"
            >
              Buy now
            </motion.button>
          </div>
        </div>
      </div>
    </motion.div>
  );
}