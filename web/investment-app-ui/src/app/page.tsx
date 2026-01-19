"use client";

import Image from "next/image";
import React, { useEffect, useState } from "react";

interface Customer {
  customerId?: string;
  CustomerId?: string;
  firstName?: string;
  FirstName?: string;
  lastName?: string;
  LastName?: string;
  email?: string;
  Email?: string;
  phone?: string;
  Phone?: string;
  createdAt?: string;
  CreatedAt?: string;
}

export default function Home() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;

    async function loadCustomers() {
      try {
        const res = await fetch("/api/customers");
        if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
        const data = await res.json();
        if (mounted) {
          setCustomers(Array.isArray(data) ? data : []);
        }
      } catch (err: any) {
        if (mounted) setError(err.message || "Failed to fetch customers");
      } finally {
        if (mounted) setLoading(false);
      }
    }

    loadCustomers();
    return () => {
      mounted = false;
    };
  }, []);

  return (
    <div className="flex min-h-screen items-start justify-center bg-zinc-50 font-sans dark:bg-black py-12">
      <main className="w-full max-w-5xl px-6">
        <div className="flex items-center gap-4 mb-8">
          <Image src="/next.svg" alt="Next.js" width={80} height={20} />
          <h1 className="text-2xl font-semibold">Customers</h1>
        </div>

        {loading ? (
          <div>Loading customers…</div>
        ) : error ? (
          <div className="text-red-600">Error: {error}</div>
        ) : (
          <div className="overflow-x-auto rounded-md border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Name</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Email</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Phone</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Created</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100 bg-white">
                {customers.map((c, i) => {
                  const id = (c as any).customerId ?? (c as any).CustomerId ?? i;
                  const first = (c as any).firstName ?? (c as any).FirstName ?? "";
                  const last = (c as any).lastName ?? (c as any).LastName ?? "";
                  const email = (c as any).email ?? (c as any).Email ?? "";
                  const phone = (c as any).phone ?? (c as any).Phone ?? "";
                  const created = (c as any).createdAt ?? (c as any).CreatedAt ?? null;

                  return (
                    <tr key={id} className="hover:bg-gray-50">
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-900">{first} {last}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">{email}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">{phone}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-500">{created ? new Date(created).toLocaleString() : "-"}</td>
                    </tr>
                  );
                })}
                {customers.length === 0 && (
                  <tr>
                    <td colSpan={4} className="px-6 py-4 text-center text-sm text-gray-500">No customers found.</td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </main>
    </div>
  );
}
