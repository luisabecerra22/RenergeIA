"use client";

import { useRef, useEffect, useCallback } from "react";

export default function TablaConScrollDoble({ children }: { children: React.ReactNode }) {
  const topRef = useRef<HTMLDivElement>(null);
  const bottomRef = useRef<HTMLDivElement>(null);
  const innerRef = useRef<HTMLDivElement>(null);
  const syncing = useRef(false);

  const syncWidth = useCallback(() => {
    if (innerRef.current && topRef.current) {
      const inner = topRef.current.firstElementChild as HTMLDivElement;
      if (inner) inner.style.width = `${innerRef.current.scrollWidth}px`;
    }
  }, []);

  useEffect(() => {
    syncWidth();
    const obs = new ResizeObserver(syncWidth);
    if (innerRef.current) obs.observe(innerRef.current);
    return () => obs.disconnect();
  }, [syncWidth, children]);

  function onTopScroll() {
    if (syncing.current) return;
    syncing.current = true;
    if (bottomRef.current && topRef.current)
      bottomRef.current.scrollLeft = topRef.current.scrollLeft;
    syncing.current = false;
  }

  function onBottomScroll() {
    if (syncing.current) return;
    syncing.current = true;
    if (topRef.current && bottomRef.current)
      topRef.current.scrollLeft = bottomRef.current.scrollLeft;
    syncing.current = false;
  }

  return (
    <>
      <div
        ref={topRef}
        onScroll={onTopScroll}
        style={{ overflowX: "auto", overflowY: "hidden", height: 16 }}
      >
        <div style={{ height: 1 }} />
      </div>
      <div
        ref={bottomRef}
        className="tabla-wrap"
        onScroll={onBottomScroll}
      >
        <div ref={innerRef}>
          {children}
        </div>
      </div>
    </>
  );
}
