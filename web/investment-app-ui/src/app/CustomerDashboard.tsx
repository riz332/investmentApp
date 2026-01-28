import React from "react";
import { Customer } from "./CustomerTable";

interface CustomerDashboardProps {
  user: Customer;
}

export default function CustomerDashboard({ user }: CustomerDashboardProps) {
  const first = user.firstName ?? user.FirstName ?? "User";
  const last = user.lastName ?? user.LastName ?? "";

  return (
    <div className="rounded-md border border-gray-200 bg-white p-6 shadow-sm">
      <h2 className="mb-4 text-xl font-semibold">Welcome, {first} {last}</h2>
      <div className="space-y-4">
        <div>
          <h3 className="text-lg font-medium">My Profile</h3>
          <div className="mt-2 space-y-1 text-sm text-gray-700">
            <p><strong>Email:</strong> {user.email ?? user.Email}</p>
            <p><strong>Phone:</strong> {user.phone ?? user.Phone ?? "N/A"}</p>
            <p><strong>Member since:</strong> {user.createdAt || user.CreatedAt ? new Date(user.createdAt! ?? user.CreatedAt!).toLocaleDateString() : "-"}</p>
          </div>
        </div>
        <div className="pt-4">
          <h3 className="text-lg font-medium">My Portfolio</h3>
          <p className="mt-2 text-sm text-gray-500">Portfolio details will be displayed here.</p>
        </div>
        <div className="pt-4">
          <h3 className="text-lg font-medium">Transactions</h3>
          <p className="mt-2 text-sm text-gray-500">Transaction history will be displayed here.</p>
        </div>
        <div className="pt-4">
          <h3 className="text-lg font-medium">History of Change</h3>
          <p className="mt-2 text-sm text-gray-500">A log of changes will be displayed here.</p>
        </div>
      </div>
    </div>
  );
}