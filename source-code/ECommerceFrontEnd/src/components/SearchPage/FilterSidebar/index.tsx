import React from "react";
import { Card, Select, Slider, Input, Collapse, Checkbox, Badge } from "antd";
import {
    DownOutlined,
    RightOutlined,
    SearchOutlined,
} from "@ant-design/icons";

const { Option } = Select;
const { Panel } = Collapse;

const FilterSidebar: React.FC = () => {
    return (
        <Card
            title="Filter By"
            style={{
                borderRadius: 8,
                border: "1px solid #f0f0f0",
                boxShadow: "0 2px 6px rgba(0,0,0,0.05)",
            }}
            bodyStyle={{ padding: "16px" }}
        >
            {/* Product Type */}
            <div style={{ marginBottom: 16 }}>
                <p style={{ fontWeight: 500, marginBottom: 4 }}>Product Type</p>
                <Select defaultValue="All" style={{ width: "100%" }}>
                    <Option value="All">All</Option>
                    <Option value="Electronics">Electronics</Option>
                    <Option value="Fashion">Fashion</Option>
                    <Option value="Home">Home & Kitchen</Option>
                </Select>
            </div>

            {/* Price Range */}
            <div style={{ marginBottom: 16 }}>
                <p style={{ fontWeight: 500, marginBottom: 4 }}>Price</p>
                <div
                    style={{
                        display: "flex",
                        justifyContent: "space-between",
                        marginBottom: 4,
                    }}
                >
                    <Input placeholder="Min" style={{ width: "45%" }} size="small" />
                    <Input placeholder="Max" style={{ width: "45%" }} size="small" />
                </div>
                <Slider range defaultValue={[0, 4000]} max={4000} />
            </div>

          
           

            {/* Brands */}
            <div style={{ marginBottom: 16 }}>
                <p style={{ fontWeight: 500, marginBottom: 8 }}>Brands</p>
                <Input
                    placeholder="Search by brands"
                    prefix={<SearchOutlined />}
                    size="small"
                />
                <div
                    style={{
                        marginTop: 8,
                        maxHeight: 160,
                        overflowY: "auto",
                        paddingRight: 8,
                    }}
                >
                    {[
                        "Kenstar",
                        "Electrical Charge",
                        "Diamond Store",
                        "Global Tech",
                        "Cool Speakers",
                        "Tech Com",
                        "Power Energy",
                    ].map((b, ) => ( // .map((b,i)
                        <div
                            key={b}
                            style={{
                                display: "flex",
                                justifyContent: "space-between",
                                marginBottom: 6,
                            }}
                        >
                            <Checkbox>{b}</Checkbox>
                            <Badge
                                count={Math.floor(Math.random() * 30) + 1}
                                style={{ backgroundColor: "#999" }}
                            />
                        </div>
                    ))}
                </div>
            </div>

            {/* Publishing House */}
            <div style={{ marginBottom: 16 }}>
                <p style={{ fontWeight: 500, marginBottom: 8 }}>Publishing House</p>
                <Input placeholder="Search by name" prefix={<SearchOutlined />} size="small" />
                <div style={{ marginTop: 8 }}>
                    <Checkbox>Unknown</Checkbox>
                    <Badge count={7} style={{ backgroundColor: "#999", marginLeft: 4 }} />
                </div>
            </div>

            {/* Author / Creator / Artist */}
            <div>
                <p style={{ fontWeight: 500, marginBottom: 8 }}>Author/Creator/Artist</p>
                <Input placeholder="Search by name" prefix={<SearchOutlined />} size="small" />
                <div style={{ marginTop: 8 }}>
                    <Checkbox>Unknown</Checkbox>
                    <Badge count={7} style={{ backgroundColor: "#999", marginLeft: 4 }} />
                </div>
            </div>
        </Card>
    );
};

export default FilterSidebar;
