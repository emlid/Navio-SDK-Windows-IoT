// C++ for the Windows Runtime vv1.0.170303.6
// Copyright (c) 2017 Microsoft Corporation. All rights reserved.

#pragma once

#include "../base.h"
#include "Windows.Perception.Spatial.0.h"
#include "Windows.Foundation.0.h"
#include "Windows.Foundation.Collections.0.h"
#include "Windows.Perception.0.h"
#include "Windows.Storage.Streams.0.h"
#include "Windows.System.RemoteSystems.0.h"
#include "Windows.Foundation.1.h"
#include "Windows.Foundation.Collections.1.h"

WINRT_EXPORT namespace winrt {

namespace ABI::Windows::Perception::Spatial {

struct SpatialBoundingFrustum
{
    Windows::Foundation::Numerics::plane Near;
    Windows::Foundation::Numerics::plane Far;
    Windows::Foundation::Numerics::plane Right;
    Windows::Foundation::Numerics::plane Left;
    Windows::Foundation::Numerics::plane Top;
    Windows::Foundation::Numerics::plane Bottom;
};

struct SpatialBoundingBox
{
    Windows::Foundation::Numerics::float3 Center;
    Windows::Foundation::Numerics::float3 Extents;
};

struct SpatialBoundingOrientedBox
{
    Windows::Foundation::Numerics::float3 Center;
    Windows::Foundation::Numerics::float3 Extents;
    Windows::Foundation::Numerics::quaternion Orientation;
};

struct SpatialBoundingSphere
{
    Windows::Foundation::Numerics::float3 Center;
    float Radius;
};

}

namespace Windows::Perception::Spatial {

using SpatialBoundingFrustum = ABI::Windows::Perception::Spatial::SpatialBoundingFrustum;
using SpatialBoundingBox = ABI::Windows::Perception::Spatial::SpatialBoundingBox;
using SpatialBoundingOrientedBox = ABI::Windows::Perception::Spatial::SpatialBoundingOrientedBox;
using SpatialBoundingSphere = ABI::Windows::Perception::Spatial::SpatialBoundingSphere;

}

namespace ABI::Windows::Perception::Spatial {

struct __declspec(uuid("0529e5ce-1d34-3702-bcec-eabff578a869")) __declspec(novtable) ISpatialAnchor : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_CoordinateSystem(Windows::Perception::Spatial::ISpatialCoordinateSystem ** value) = 0;
    virtual HRESULT __stdcall get_RawCoordinateSystem(Windows::Perception::Spatial::ISpatialCoordinateSystem ** value) = 0;
    virtual HRESULT __stdcall add_RawCoordinateSystemAdjusted(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialAnchor, Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs> * handler, event_token * cookie) = 0;
    virtual HRESULT __stdcall remove_RawCoordinateSystemAdjusted(event_token cookie) = 0;
};

struct __declspec(uuid("ed17c908-a695-4cf6-92fd-97263ba71047")) __declspec(novtable) ISpatialAnchor2 : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_RemovedByUser(bool * value) = 0;
};

struct __declspec(uuid("88e30eab-f3b7-420b-b086-8a80c07d910d")) __declspec(novtable) ISpatialAnchorManagerStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_RequestStoreAsync(Windows::Foundation::IAsyncOperation<Windows::Perception::Spatial::SpatialAnchorStore> ** value) = 0;
};

struct __declspec(uuid("a1e81eb8-56c7-3117-a2e4-81e0fcf28e00")) __declspec(novtable) ISpatialAnchorRawCoordinateSystemAdjustedEventArgs : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_OldRawCoordinateSystemToNewRawCoordinateSystemTransform(Windows::Foundation::Numerics::float4x4 * value) = 0;
};

struct __declspec(uuid("a9928642-0174-311c-ae79-0e5107669f16")) __declspec(novtable) ISpatialAnchorStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_TryCreateRelativeTo(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::ISpatialAnchor ** value) = 0;
    virtual HRESULT __stdcall abi_TryCreateWithPositionRelativeTo(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Foundation::Numerics::float3 position, Windows::Perception::Spatial::ISpatialAnchor ** value) = 0;
    virtual HRESULT __stdcall abi_TryCreateWithPositionAndOrientationRelativeTo(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Foundation::Numerics::float3 position, Windows::Foundation::Numerics::quaternion orientation, Windows::Perception::Spatial::ISpatialAnchor ** value) = 0;
};

struct __declspec(uuid("b0bc3636-486a-3cb0-9e6f-1245165c4db6")) __declspec(novtable) ISpatialAnchorStore : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_GetAllSavedAnchors(Windows::Foundation::Collections::IMapView<hstring, Windows::Perception::Spatial::SpatialAnchor> ** value) = 0;
    virtual HRESULT __stdcall abi_TrySave(hstring id, Windows::Perception::Spatial::ISpatialAnchor * anchor, bool * succeeded) = 0;
    virtual HRESULT __stdcall abi_Remove(hstring id) = 0;
    virtual HRESULT __stdcall abi_Clear() = 0;
};

struct __declspec(uuid("03bbf9b9-12d8-4bce-8835-c5df3ac0adab")) __declspec(novtable) ISpatialAnchorTransferManagerStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_TryImportAnchorsAsync(Windows::Storage::Streams::IInputStream * stream, Windows::Foundation::IAsyncOperation<Windows::Foundation::Collections::IMapView<hstring, Windows::Perception::Spatial::SpatialAnchor>> ** operation) = 0;
    virtual HRESULT __stdcall abi_TryExportAnchorsAsync(Windows::Foundation::Collections::IIterable<Windows::Foundation::Collections::IKeyValuePair<hstring, Windows::Perception::Spatial::SpatialAnchor>> * anchors, Windows::Storage::Streams::IOutputStream * stream, Windows::Foundation::IAsyncOperation<bool> ** operation) = 0;
    virtual HRESULT __stdcall abi_RequestAccessAsync(Windows::Foundation::IAsyncOperation<winrt::Windows::Perception::Spatial::SpatialPerceptionAccessStatus> ** result) = 0;
};

struct __declspec(uuid("fb2065da-68c3-33df-b7af-4c787207999c")) __declspec(novtable) ISpatialBoundingVolume : Windows::Foundation::IInspectable
{
};

struct __declspec(uuid("05889117-b3e1-36d8-b017-566181a5b196")) __declspec(novtable) ISpatialBoundingVolumeStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_FromBox(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::SpatialBoundingBox box, Windows::Perception::Spatial::ISpatialBoundingVolume ** value) = 0;
    virtual HRESULT __stdcall abi_FromOrientedBox(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::SpatialBoundingOrientedBox box, Windows::Perception::Spatial::ISpatialBoundingVolume ** value) = 0;
    virtual HRESULT __stdcall abi_FromSphere(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::SpatialBoundingSphere sphere, Windows::Perception::Spatial::ISpatialBoundingVolume ** value) = 0;
    virtual HRESULT __stdcall abi_FromFrustum(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::SpatialBoundingFrustum frustum, Windows::Perception::Spatial::ISpatialBoundingVolume ** value) = 0;
};

struct __declspec(uuid("69ebca4b-60a3-3586-a653-59a7bd676d07")) __declspec(novtable) ISpatialCoordinateSystem : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_TryGetTransformTo(Windows::Perception::Spatial::ISpatialCoordinateSystem * target, Windows::Foundation::IReference<Windows::Foundation::Numerics::float4x4> ** value) = 0;
};

struct __declspec(uuid("166de955-e1eb-454c-ba08-e6c0668ddc65")) __declspec(novtable) ISpatialEntity : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Id(hstring * value) = 0;
    virtual HRESULT __stdcall get_Anchor(Windows::Perception::Spatial::ISpatialAnchor ** value) = 0;
    virtual HRESULT __stdcall get_Properties(Windows::Foundation::Collections::IPropertySet ** value) = 0;
};

struct __declspec(uuid("a397f49b-156a-4707-ac2c-d31d570ed399")) __declspec(novtable) ISpatialEntityAddedEventArgs : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Entity(Windows::Perception::Spatial::ISpatialEntity ** value) = 0;
};

struct __declspec(uuid("e1f1e325-349f-4225-a2f3-4b01c15fe056")) __declspec(novtable) ISpatialEntityFactory : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_CreateWithSpatialAnchor(Windows::Perception::Spatial::ISpatialAnchor * spatialAnchor, Windows::Perception::Spatial::ISpatialEntity ** value) = 0;
    virtual HRESULT __stdcall abi_CreateWithSpatialAnchorAndProperties(Windows::Perception::Spatial::ISpatialAnchor * spatialAnchor, Windows::Foundation::Collections::IPropertySet * propertySet, Windows::Perception::Spatial::ISpatialEntity ** value) = 0;
};

struct __declspec(uuid("91741800-536d-4e9f-abf6-415b5444d651")) __declspec(novtable) ISpatialEntityRemovedEventArgs : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Entity(Windows::Perception::Spatial::ISpatialEntity ** value) = 0;
};

struct __declspec(uuid("329788ba-e513-4f06-889d-1be30ecf43e6")) __declspec(novtable) ISpatialEntityStore : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_SaveAsync(Windows::Perception::Spatial::ISpatialEntity * entity, Windows::Foundation::IAsyncAction ** action) = 0;
    virtual HRESULT __stdcall abi_RemoveAsync(Windows::Perception::Spatial::ISpatialEntity * entity, Windows::Foundation::IAsyncAction ** action) = 0;
    virtual HRESULT __stdcall abi_CreateEntityWatcher(Windows::Perception::Spatial::ISpatialEntityWatcher ** value) = 0;
};

struct __declspec(uuid("6b4b389e-7c50-4e92-8a62-4d1d4b7ccd3e")) __declspec(novtable) ISpatialEntityStoreStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_IsSupported(bool * value) = 0;
    virtual HRESULT __stdcall abi_TryGetForRemoteSystemSession(Windows::System::RemoteSystems::IRemoteSystemSession * session, Windows::Perception::Spatial::ISpatialEntityStore ** value) = 0;
};

struct __declspec(uuid("e5671766-627b-43cb-a49f-b3be6d47deed")) __declspec(novtable) ISpatialEntityUpdatedEventArgs : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Entity(Windows::Perception::Spatial::ISpatialEntity ** value) = 0;
};

struct __declspec(uuid("b3b85fa0-6d5e-4bbc-805d-5fe5b9ba1959")) __declspec(novtable) ISpatialEntityWatcher : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Status(winrt::Windows::Perception::Spatial::SpatialEntityWatcherStatus * value) = 0;
    virtual HRESULT __stdcall add_Added(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityAddedEventArgs> * handler, event_token * token) = 0;
    virtual HRESULT __stdcall remove_Added(event_token token) = 0;
    virtual HRESULT __stdcall add_Updated(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs> * handler, event_token * token) = 0;
    virtual HRESULT __stdcall remove_Updated(event_token token) = 0;
    virtual HRESULT __stdcall add_Removed(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityRemovedEventArgs> * handler, event_token * token) = 0;
    virtual HRESULT __stdcall remove_Removed(event_token token) = 0;
    virtual HRESULT __stdcall add_EnumerationCompleted(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Foundation::IInspectable> * handler, event_token * token) = 0;
    virtual HRESULT __stdcall remove_EnumerationCompleted(event_token token) = 0;
    virtual HRESULT __stdcall abi_Start() = 0;
    virtual HRESULT __stdcall abi_Stop() = 0;
};

struct __declspec(uuid("1d81d29d-24a1-37d5-8fa1-39b4f9ad67e2")) __declspec(novtable) ISpatialLocation : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Position(Windows::Foundation::Numerics::float3 * value) = 0;
    virtual HRESULT __stdcall get_Orientation(Windows::Foundation::Numerics::quaternion * value) = 0;
    virtual HRESULT __stdcall get_AbsoluteLinearVelocity(Windows::Foundation::Numerics::float3 * value) = 0;
    virtual HRESULT __stdcall get_AbsoluteLinearAcceleration(Windows::Foundation::Numerics::float3 * value) = 0;
    virtual HRESULT __stdcall get_AbsoluteAngularVelocity(Windows::Foundation::Numerics::quaternion * value) = 0;
    virtual HRESULT __stdcall get_AbsoluteAngularAcceleration(Windows::Foundation::Numerics::quaternion * value) = 0;
};

struct __declspec(uuid("f6478925-9e0c-3bb6-997e-b64ecca24cf4")) __declspec(novtable) ISpatialLocator : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Locatability(winrt::Windows::Perception::Spatial::SpatialLocatability * value) = 0;
    virtual HRESULT __stdcall add_LocatabilityChanged(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Foundation::IInspectable> * handler, event_token * cookie) = 0;
    virtual HRESULT __stdcall remove_LocatabilityChanged(event_token cookie) = 0;
    virtual HRESULT __stdcall add_PositionalTrackingDeactivating(Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs> * handler, event_token * cookie) = 0;
    virtual HRESULT __stdcall remove_PositionalTrackingDeactivating(event_token cookie) = 0;
    virtual HRESULT __stdcall abi_TryLocateAtTimestamp(Windows::Perception::IPerceptionTimestamp * timestamp, Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, Windows::Perception::Spatial::ISpatialLocation ** value) = 0;
    virtual HRESULT __stdcall abi_CreateAttachedFrameOfReferenceAtCurrentHeading(Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateAttachedFrameOfReferenceAtCurrentHeadingWithPosition(Windows::Foundation::Numerics::float3 relativePosition, Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateAttachedFrameOfReferenceAtCurrentHeadingWithPositionAndOrientation(Windows::Foundation::Numerics::float3 relativePosition, Windows::Foundation::Numerics::quaternion relativeOrientation, Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateAttachedFrameOfReferenceAtCurrentHeadingWithPositionAndOrientationAndRelativeHeading(Windows::Foundation::Numerics::float3 relativePosition, Windows::Foundation::Numerics::quaternion relativeOrientation, double relativeHeadingInRadians, Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateStationaryFrameOfReferenceAtCurrentLocation(Windows::Perception::Spatial::ISpatialStationaryFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateStationaryFrameOfReferenceAtCurrentLocationWithPosition(Windows::Foundation::Numerics::float3 relativePosition, Windows::Perception::Spatial::ISpatialStationaryFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateStationaryFrameOfReferenceAtCurrentLocationWithPositionAndOrientation(Windows::Foundation::Numerics::float3 relativePosition, Windows::Foundation::Numerics::quaternion relativeOrientation, Windows::Perception::Spatial::ISpatialStationaryFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall abi_CreateStationaryFrameOfReferenceAtCurrentLocationWithPositionAndOrientationAndRelativeHeading(Windows::Foundation::Numerics::float3 relativePosition, Windows::Foundation::Numerics::quaternion relativeOrientation, double relativeHeadingInRadians, Windows::Perception::Spatial::ISpatialStationaryFrameOfReference ** value) = 0;
};

struct __declspec(uuid("e1774ef6-1f4f-499c-9625-ef5e6ed7a048")) __declspec(novtable) ISpatialLocatorAttachedFrameOfReference : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_RelativePosition(Windows::Foundation::Numerics::float3 * value) = 0;
    virtual HRESULT __stdcall put_RelativePosition(Windows::Foundation::Numerics::float3 value) = 0;
    virtual HRESULT __stdcall get_RelativeOrientation(Windows::Foundation::Numerics::quaternion * value) = 0;
    virtual HRESULT __stdcall put_RelativeOrientation(Windows::Foundation::Numerics::quaternion value) = 0;
    virtual HRESULT __stdcall abi_AdjustHeading(double headingOffsetInRadians) = 0;
    virtual HRESULT __stdcall abi_GetStationaryCoordinateSystemAtTimestamp(Windows::Perception::IPerceptionTimestamp * timestamp, Windows::Perception::Spatial::ISpatialCoordinateSystem ** value) = 0;
    virtual HRESULT __stdcall abi_TryGetRelativeHeadingAtTimestamp(Windows::Perception::IPerceptionTimestamp * timestamp, Windows::Foundation::IReference<double> ** value) = 0;
};

struct __declspec(uuid("b8a84063-e3f4-368b-9061-9ea9d1d6cc16")) __declspec(novtable) ISpatialLocatorPositionalTrackingDeactivatingEventArgs : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Canceled(bool * value) = 0;
    virtual HRESULT __stdcall put_Canceled(bool value) = 0;
};

struct __declspec(uuid("b76e3340-a7c2-361b-bb82-56e93b89b1bb")) __declspec(novtable) ISpatialLocatorStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall abi_GetDefault(Windows::Perception::Spatial::ISpatialLocator ** value) = 0;
};

struct __declspec(uuid("7a8a3464-ad0d-4590-ab86-33062b674926")) __declspec(novtable) ISpatialStageFrameOfReference : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_CoordinateSystem(Windows::Perception::Spatial::ISpatialCoordinateSystem ** value) = 0;
    virtual HRESULT __stdcall get_MovementRange(winrt::Windows::Perception::Spatial::SpatialMovementRange * value) = 0;
    virtual HRESULT __stdcall get_LookDirectionRange(winrt::Windows::Perception::Spatial::SpatialLookDirectionRange * value) = 0;
    virtual HRESULT __stdcall abi_GetCoordinateSystemAtCurrentLocation(Windows::Perception::Spatial::ISpatialLocator * locator, Windows::Perception::Spatial::ISpatialCoordinateSystem ** result) = 0;
    virtual HRESULT __stdcall abi_TryGetMovementBounds(Windows::Perception::Spatial::ISpatialCoordinateSystem * coordinateSystem, uint32_t * __valueSize, Windows::Foundation::Numerics::float3 ** value) = 0;
};

struct __declspec(uuid("f78d5c4d-a0a4-499c-8d91-a8c965d40654")) __declspec(novtable) ISpatialStageFrameOfReferenceStatics : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_Current(Windows::Perception::Spatial::ISpatialStageFrameOfReference ** value) = 0;
    virtual HRESULT __stdcall add_CurrentChanged(Windows::Foundation::EventHandler<Windows::Foundation::IInspectable> * handler, event_token * cookie) = 0;
    virtual HRESULT __stdcall remove_CurrentChanged(event_token cookie) = 0;
    virtual HRESULT __stdcall abi_RequestNewStageAsync(Windows::Foundation::IAsyncOperation<Windows::Perception::Spatial::SpatialStageFrameOfReference> ** result) = 0;
};

struct __declspec(uuid("09dbccb9-bcf8-3e7f-be7e-7edccbb178a8")) __declspec(novtable) ISpatialStationaryFrameOfReference : Windows::Foundation::IInspectable
{
    virtual HRESULT __stdcall get_CoordinateSystem(Windows::Perception::Spatial::ISpatialCoordinateSystem ** value) = 0;
};

}

namespace ABI {

template <> struct traits<Windows::Perception::Spatial::SpatialAnchor> { using default_interface = Windows::Perception::Spatial::ISpatialAnchor; };
template <> struct traits<Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs> { using default_interface = Windows::Perception::Spatial::ISpatialAnchorRawCoordinateSystemAdjustedEventArgs; };
template <> struct traits<Windows::Perception::Spatial::SpatialAnchorStore> { using default_interface = Windows::Perception::Spatial::ISpatialAnchorStore; };
template <> struct traits<Windows::Perception::Spatial::SpatialBoundingVolume> { using default_interface = Windows::Perception::Spatial::ISpatialBoundingVolume; };
template <> struct traits<Windows::Perception::Spatial::SpatialCoordinateSystem> { using default_interface = Windows::Perception::Spatial::ISpatialCoordinateSystem; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntity> { using default_interface = Windows::Perception::Spatial::ISpatialEntity; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntityAddedEventArgs> { using default_interface = Windows::Perception::Spatial::ISpatialEntityAddedEventArgs; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntityRemovedEventArgs> { using default_interface = Windows::Perception::Spatial::ISpatialEntityRemovedEventArgs; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntityStore> { using default_interface = Windows::Perception::Spatial::ISpatialEntityStore; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs> { using default_interface = Windows::Perception::Spatial::ISpatialEntityUpdatedEventArgs; };
template <> struct traits<Windows::Perception::Spatial::SpatialEntityWatcher> { using default_interface = Windows::Perception::Spatial::ISpatialEntityWatcher; };
template <> struct traits<Windows::Perception::Spatial::SpatialLocation> { using default_interface = Windows::Perception::Spatial::ISpatialLocation; };
template <> struct traits<Windows::Perception::Spatial::SpatialLocator> { using default_interface = Windows::Perception::Spatial::ISpatialLocator; };
template <> struct traits<Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference> { using default_interface = Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference; };
template <> struct traits<Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs> { using default_interface = Windows::Perception::Spatial::ISpatialLocatorPositionalTrackingDeactivatingEventArgs; };
template <> struct traits<Windows::Perception::Spatial::SpatialStageFrameOfReference> { using default_interface = Windows::Perception::Spatial::ISpatialStageFrameOfReference; };
template <> struct traits<Windows::Perception::Spatial::SpatialStationaryFrameOfReference> { using default_interface = Windows::Perception::Spatial::ISpatialStationaryFrameOfReference; };

}

namespace Windows::Perception::Spatial {

template <typename D>
struct WINRT_EBO impl_ISpatialAnchor
{
    Windows::Perception::Spatial::SpatialCoordinateSystem CoordinateSystem() const;
    Windows::Perception::Spatial::SpatialCoordinateSystem RawCoordinateSystem() const;
    event_token RawCoordinateSystemAdjusted(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialAnchor, Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs> & handler) const;
    using RawCoordinateSystemAdjusted_revoker = event_revoker<ISpatialAnchor>;
    RawCoordinateSystemAdjusted_revoker RawCoordinateSystemAdjusted(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialAnchor, Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs> & handler) const;
    void RawCoordinateSystemAdjusted(event_token cookie) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchor2
{
    bool RemovedByUser() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchorManagerStatics
{
    Windows::Foundation::IAsyncOperation<Windows::Perception::Spatial::SpatialAnchorStore> RequestStoreAsync() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchorRawCoordinateSystemAdjustedEventArgs
{
    Windows::Foundation::Numerics::float4x4 OldRawCoordinateSystemToNewRawCoordinateSystemTransform() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchorStatics
{
    Windows::Perception::Spatial::SpatialAnchor TryCreateRelativeTo(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem) const;
    Windows::Perception::Spatial::SpatialAnchor TryCreateRelativeTo(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Foundation::Numerics::float3 & position) const;
    Windows::Perception::Spatial::SpatialAnchor TryCreateRelativeTo(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Foundation::Numerics::float3 & position, const Windows::Foundation::Numerics::quaternion & orientation) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchorStore
{
    Windows::Foundation::Collections::IMapView<hstring, Windows::Perception::Spatial::SpatialAnchor> GetAllSavedAnchors() const;
    bool TrySave(hstring_view id, const Windows::Perception::Spatial::SpatialAnchor & anchor) const;
    void Remove(hstring_view id) const;
    void Clear() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialAnchorTransferManagerStatics
{
    [[deprecated("Use SpatialEntityStore instead of SpatialAnchorTransferManager. For more info, see MSDN.")]] Windows::Foundation::IAsyncOperation<Windows::Foundation::Collections::IMapView<hstring, Windows::Perception::Spatial::SpatialAnchor>> TryImportAnchorsAsync(const Windows::Storage::Streams::IInputStream & stream) const;
    [[deprecated("Use SpatialEntityStore instead of SpatialAnchorTransferManager. For more info, see MSDN.")]] Windows::Foundation::IAsyncOperation<bool> TryExportAnchorsAsync(iterable<Windows::Foundation::Collections::IKeyValuePair<hstring, Windows::Perception::Spatial::SpatialAnchor>> anchors, const Windows::Storage::Streams::IOutputStream & stream) const;
    [[deprecated("Use SpatialEntityStore instead of SpatialAnchorTransferManager. For more info, see MSDN.")]] Windows::Foundation::IAsyncOperation<winrt::Windows::Perception::Spatial::SpatialPerceptionAccessStatus> RequestAccessAsync() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialBoundingVolume
{
};

template <typename D>
struct WINRT_EBO impl_ISpatialBoundingVolumeStatics
{
    Windows::Perception::Spatial::SpatialBoundingVolume FromBox(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Perception::Spatial::SpatialBoundingBox & box) const;
    Windows::Perception::Spatial::SpatialBoundingVolume FromOrientedBox(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Perception::Spatial::SpatialBoundingOrientedBox & box) const;
    Windows::Perception::Spatial::SpatialBoundingVolume FromSphere(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Perception::Spatial::SpatialBoundingSphere & sphere) const;
    Windows::Perception::Spatial::SpatialBoundingVolume FromFrustum(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem, const Windows::Perception::Spatial::SpatialBoundingFrustum & frustum) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialCoordinateSystem
{
    Windows::Foundation::IReference<Windows::Foundation::Numerics::float4x4> TryGetTransformTo(const Windows::Perception::Spatial::SpatialCoordinateSystem & target) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntity
{
    hstring Id() const;
    Windows::Perception::Spatial::SpatialAnchor Anchor() const;
    Windows::Foundation::Collections::ValueSet Properties() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityAddedEventArgs
{
    Windows::Perception::Spatial::SpatialEntity Entity() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityFactory
{
    Windows::Perception::Spatial::SpatialEntity CreateWithSpatialAnchor(const Windows::Perception::Spatial::SpatialAnchor & spatialAnchor) const;
    Windows::Perception::Spatial::SpatialEntity CreateWithSpatialAnchorAndProperties(const Windows::Perception::Spatial::SpatialAnchor & spatialAnchor, const Windows::Foundation::Collections::ValueSet & propertySet) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityRemovedEventArgs
{
    Windows::Perception::Spatial::SpatialEntity Entity() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityStore
{
    Windows::Foundation::IAsyncAction SaveAsync(const Windows::Perception::Spatial::SpatialEntity & entity) const;
    Windows::Foundation::IAsyncAction RemoveAsync(const Windows::Perception::Spatial::SpatialEntity & entity) const;
    Windows::Perception::Spatial::SpatialEntityWatcher CreateEntityWatcher() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityStoreStatics
{
    bool IsSupported() const;
    Windows::Perception::Spatial::SpatialEntityStore TryGet(const Windows::System::RemoteSystems::RemoteSystemSession & session) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityUpdatedEventArgs
{
    Windows::Perception::Spatial::SpatialEntity Entity() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialEntityWatcher
{
    Windows::Perception::Spatial::SpatialEntityWatcherStatus Status() const;
    event_token Added(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityAddedEventArgs> & handler) const;
    using Added_revoker = event_revoker<ISpatialEntityWatcher>;
    Added_revoker Added(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityAddedEventArgs> & handler) const;
    void Added(event_token token) const;
    event_token Updated(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs> & handler) const;
    using Updated_revoker = event_revoker<ISpatialEntityWatcher>;
    Updated_revoker Updated(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs> & handler) const;
    void Updated(event_token token) const;
    event_token Removed(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityRemovedEventArgs> & handler) const;
    using Removed_revoker = event_revoker<ISpatialEntityWatcher>;
    Removed_revoker Removed(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Perception::Spatial::SpatialEntityRemovedEventArgs> & handler) const;
    void Removed(event_token token) const;
    event_token EnumerationCompleted(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Foundation::IInspectable> & handler) const;
    using EnumerationCompleted_revoker = event_revoker<ISpatialEntityWatcher>;
    EnumerationCompleted_revoker EnumerationCompleted(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialEntityWatcher, Windows::Foundation::IInspectable> & handler) const;
    void EnumerationCompleted(event_token token) const;
    void Start() const;
    void Stop() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialLocation
{
    Windows::Foundation::Numerics::float3 Position() const;
    Windows::Foundation::Numerics::quaternion Orientation() const;
    Windows::Foundation::Numerics::float3 AbsoluteLinearVelocity() const;
    Windows::Foundation::Numerics::float3 AbsoluteLinearAcceleration() const;
    Windows::Foundation::Numerics::quaternion AbsoluteAngularVelocity() const;
    Windows::Foundation::Numerics::quaternion AbsoluteAngularAcceleration() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialLocator
{
    Windows::Perception::Spatial::SpatialLocatability Locatability() const;
    event_token LocatabilityChanged(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Foundation::IInspectable> & handler) const;
    using LocatabilityChanged_revoker = event_revoker<ISpatialLocator>;
    LocatabilityChanged_revoker LocatabilityChanged(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Foundation::IInspectable> & handler) const;
    void LocatabilityChanged(event_token cookie) const;
    event_token PositionalTrackingDeactivating(const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs> & handler) const;
    using PositionalTrackingDeactivating_revoker = event_revoker<ISpatialLocator>;
    PositionalTrackingDeactivating_revoker PositionalTrackingDeactivating(auto_revoke_t, const Windows::Foundation::TypedEventHandler<Windows::Perception::Spatial::SpatialLocator, Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs> & handler) const;
    void PositionalTrackingDeactivating(event_token cookie) const;
    Windows::Perception::Spatial::SpatialLocation TryLocateAtTimestamp(const Windows::Perception::PerceptionTimestamp & timestamp, const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem) const;
    Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference CreateAttachedFrameOfReferenceAtCurrentHeading() const;
    Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference CreateAttachedFrameOfReferenceAtCurrentHeading(const Windows::Foundation::Numerics::float3 & relativePosition) const;
    Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference CreateAttachedFrameOfReferenceAtCurrentHeading(const Windows::Foundation::Numerics::float3 & relativePosition, const Windows::Foundation::Numerics::quaternion & relativeOrientation) const;
    Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference CreateAttachedFrameOfReferenceAtCurrentHeading(const Windows::Foundation::Numerics::float3 & relativePosition, const Windows::Foundation::Numerics::quaternion & relativeOrientation, double relativeHeadingInRadians) const;
    Windows::Perception::Spatial::SpatialStationaryFrameOfReference CreateStationaryFrameOfReferenceAtCurrentLocation() const;
    Windows::Perception::Spatial::SpatialStationaryFrameOfReference CreateStationaryFrameOfReferenceAtCurrentLocation(const Windows::Foundation::Numerics::float3 & relativePosition) const;
    Windows::Perception::Spatial::SpatialStationaryFrameOfReference CreateStationaryFrameOfReferenceAtCurrentLocation(const Windows::Foundation::Numerics::float3 & relativePosition, const Windows::Foundation::Numerics::quaternion & relativeOrientation) const;
    Windows::Perception::Spatial::SpatialStationaryFrameOfReference CreateStationaryFrameOfReferenceAtCurrentLocation(const Windows::Foundation::Numerics::float3 & relativePosition, const Windows::Foundation::Numerics::quaternion & relativeOrientation, double relativeHeadingInRadians) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialLocatorAttachedFrameOfReference
{
    Windows::Foundation::Numerics::float3 RelativePosition() const;
    void RelativePosition(const Windows::Foundation::Numerics::float3 & value) const;
    Windows::Foundation::Numerics::quaternion RelativeOrientation() const;
    void RelativeOrientation(const Windows::Foundation::Numerics::quaternion & value) const;
    void AdjustHeading(double headingOffsetInRadians) const;
    Windows::Perception::Spatial::SpatialCoordinateSystem GetStationaryCoordinateSystemAtTimestamp(const Windows::Perception::PerceptionTimestamp & timestamp) const;
    Windows::Foundation::IReference<double> TryGetRelativeHeadingAtTimestamp(const Windows::Perception::PerceptionTimestamp & timestamp) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialLocatorPositionalTrackingDeactivatingEventArgs
{
    bool Canceled() const;
    void Canceled(bool value) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialLocatorStatics
{
    Windows::Perception::Spatial::SpatialLocator GetDefault() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialStageFrameOfReference
{
    Windows::Perception::Spatial::SpatialCoordinateSystem CoordinateSystem() const;
    Windows::Perception::Spatial::SpatialMovementRange MovementRange() const;
    Windows::Perception::Spatial::SpatialLookDirectionRange LookDirectionRange() const;
    Windows::Perception::Spatial::SpatialCoordinateSystem GetCoordinateSystemAtCurrentLocation(const Windows::Perception::Spatial::SpatialLocator & locator) const;
    com_array<Windows::Foundation::Numerics::float3> TryGetMovementBounds(const Windows::Perception::Spatial::SpatialCoordinateSystem & coordinateSystem) const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialStageFrameOfReferenceStatics
{
    Windows::Perception::Spatial::SpatialStageFrameOfReference Current() const;
    event_token CurrentChanged(const Windows::Foundation::EventHandler<Windows::Foundation::IInspectable> & handler) const;
    using CurrentChanged_revoker = event_revoker<ISpatialStageFrameOfReferenceStatics>;
    CurrentChanged_revoker CurrentChanged(auto_revoke_t, const Windows::Foundation::EventHandler<Windows::Foundation::IInspectable> & handler) const;
    void CurrentChanged(event_token cookie) const;
    Windows::Foundation::IAsyncOperation<Windows::Perception::Spatial::SpatialStageFrameOfReference> RequestNewStageAsync() const;
};

template <typename D>
struct WINRT_EBO impl_ISpatialStationaryFrameOfReference
{
    Windows::Perception::Spatial::SpatialCoordinateSystem CoordinateSystem() const;
};

}

namespace impl {

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchor>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchor;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchor<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchor2>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchor2;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchor2<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchorManagerStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchorManagerStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchorManagerStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchorRawCoordinateSystemAdjustedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchorRawCoordinateSystemAdjustedEventArgs;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchorRawCoordinateSystemAdjustedEventArgs<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchorStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchorStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchorStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchorStore>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchorStore;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchorStore<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialAnchorTransferManagerStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialAnchorTransferManagerStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialAnchorTransferManagerStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialBoundingVolume>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialBoundingVolume;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialBoundingVolume<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialBoundingVolumeStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialBoundingVolumeStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialBoundingVolumeStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialCoordinateSystem>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialCoordinateSystem;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialCoordinateSystem<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntity>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntity;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntity<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityAddedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityAddedEventArgs;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityAddedEventArgs<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityFactory>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityFactory;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityFactory<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityRemovedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityRemovedEventArgs;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityRemovedEventArgs<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityStore>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityStore;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityStore<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityStoreStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityStoreStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityStoreStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityUpdatedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityUpdatedEventArgs;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityUpdatedEventArgs<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialEntityWatcher>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialEntityWatcher;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialEntityWatcher<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialLocation>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialLocation;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialLocation<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialLocator>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialLocator;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialLocator<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialLocatorAttachedFrameOfReference;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialLocatorAttachedFrameOfReference<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialLocatorPositionalTrackingDeactivatingEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialLocatorPositionalTrackingDeactivatingEventArgs;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialLocatorPositionalTrackingDeactivatingEventArgs<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialLocatorStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialLocatorStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialLocatorStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialStageFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialStageFrameOfReference;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialStageFrameOfReference<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialStageFrameOfReferenceStatics>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialStageFrameOfReferenceStatics;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialStageFrameOfReferenceStatics<D>;
};

template <> struct traits<Windows::Perception::Spatial::ISpatialStationaryFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::ISpatialStationaryFrameOfReference;
    template <typename D> using consume = Windows::Perception::Spatial::impl_ISpatialStationaryFrameOfReference<D>;
};

template <> struct traits<Windows::Perception::Spatial::SpatialAnchor>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialAnchor;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialAnchor"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialAnchorManager>
{
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialAnchorManager"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialAnchorRawCoordinateSystemAdjustedEventArgs;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialAnchorRawCoordinateSystemAdjustedEventArgs"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialAnchorStore>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialAnchorStore;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialAnchorStore"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialAnchorTransferManager>
{
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialAnchorTransferManager"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialBoundingVolume>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialBoundingVolume;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialBoundingVolume"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialCoordinateSystem>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialCoordinateSystem;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialCoordinateSystem"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntity>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntity;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntity"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntityAddedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntityAddedEventArgs;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntityAddedEventArgs"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntityRemovedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntityRemovedEventArgs;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntityRemovedEventArgs"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntityStore>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntityStore;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntityStore"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntityUpdatedEventArgs;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntityUpdatedEventArgs"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialEntityWatcher>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialEntityWatcher;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialEntityWatcher"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialLocation>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialLocation;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialLocation"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialLocator>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialLocator;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialLocator"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialLocatorAttachedFrameOfReference;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialLocatorAttachedFrameOfReference"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialLocatorPositionalTrackingDeactivatingEventArgs;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialLocatorPositionalTrackingDeactivatingEventArgs"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialStageFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialStageFrameOfReference;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialStageFrameOfReference"; }
};

template <> struct traits<Windows::Perception::Spatial::SpatialStationaryFrameOfReference>
{
    using abi = ABI::Windows::Perception::Spatial::SpatialStationaryFrameOfReference;
    static constexpr const wchar_t * name() noexcept { return L"Windows.Perception.Spatial.SpatialStationaryFrameOfReference"; }
};

}

}
