// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: runtime_debugger_log.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_runtime_5fdebugger_5flog_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_runtime_5fdebugger_5flog_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3021000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3021012 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/metadata_lite.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/generated_enum_reflection.h>
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_runtime_5fdebugger_5flog_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_runtime_5fdebugger_5flog_2eproto {
  static const uint32_t offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_runtime_5fdebugger_5flog_2eproto;
namespace RuntimeDebugger {
class LogList;
struct LogListDefaultTypeInternal;
extern LogListDefaultTypeInternal _LogList_default_instance_;
class LogNode;
struct LogNodeDefaultTypeInternal;
extern LogNodeDefaultTypeInternal _LogNode_default_instance_;
}  // namespace RuntimeDebugger
PROTOBUF_NAMESPACE_OPEN
template<> ::RuntimeDebugger::LogList* Arena::CreateMaybeMessage<::RuntimeDebugger::LogList>(Arena*);
template<> ::RuntimeDebugger::LogNode* Arena::CreateMaybeMessage<::RuntimeDebugger::LogNode>(Arena*);
PROTOBUF_NAMESPACE_CLOSE
namespace RuntimeDebugger {

enum LogType : int {
  Error = 0,
  Assert = 1,
  Warning = 2,
  Log = 3,
  Exception = 4,
  LogType_INT_MIN_SENTINEL_DO_NOT_USE_ = std::numeric_limits<int32_t>::min(),
  LogType_INT_MAX_SENTINEL_DO_NOT_USE_ = std::numeric_limits<int32_t>::max()
};
bool LogType_IsValid(int value);
constexpr LogType LogType_MIN = Error;
constexpr LogType LogType_MAX = Exception;
constexpr int LogType_ARRAYSIZE = LogType_MAX + 1;

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* LogType_descriptor();
template<typename T>
inline const std::string& LogType_Name(T enum_t_value) {
  static_assert(::std::is_same<T, LogType>::value ||
    ::std::is_integral<T>::value,
    "Incorrect type passed to function LogType_Name.");
  return ::PROTOBUF_NAMESPACE_ID::internal::NameOfEnum(
    LogType_descriptor(), enum_t_value);
}
inline bool LogType_Parse(
    ::PROTOBUF_NAMESPACE_ID::ConstStringParam name, LogType* value) {
  return ::PROTOBUF_NAMESPACE_ID::internal::ParseNamedEnum<LogType>(
    LogType_descriptor(), name, value);
}
// ===================================================================

class LogNode final :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:RuntimeDebugger.LogNode) */ {
 public:
  inline LogNode() : LogNode(nullptr) {}
  ~LogNode() override;
  explicit PROTOBUF_CONSTEXPR LogNode(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  LogNode(const LogNode& from);
  LogNode(LogNode&& from) noexcept
    : LogNode() {
    *this = ::std::move(from);
  }

  inline LogNode& operator=(const LogNode& from) {
    CopyFrom(from);
    return *this;
  }
  inline LogNode& operator=(LogNode&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const LogNode& default_instance() {
    return *internal_default_instance();
  }
  static inline const LogNode* internal_default_instance() {
    return reinterpret_cast<const LogNode*>(
               &_LogNode_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  friend void swap(LogNode& a, LogNode& b) {
    a.Swap(&b);
  }
  inline void Swap(LogNode* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(LogNode* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  LogNode* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<LogNode>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::Message::CopyFrom;
  void CopyFrom(const LogNode& from);
  using ::PROTOBUF_NAMESPACE_ID::Message::MergeFrom;
  void MergeFrom( const LogNode& from) {
    LogNode::MergeImpl(*this, from);
  }
  private:
  static void MergeImpl(::PROTOBUF_NAMESPACE_ID::Message& to_msg, const ::PROTOBUF_NAMESPACE_ID::Message& from_msg);
  public:
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  uint8_t* _InternalSerialize(
      uint8_t* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _impl_._cached_size_.Get(); }

  private:
  void SharedCtor(::PROTOBUF_NAMESPACE_ID::Arena* arena, bool is_message_owned);
  void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(LogNode* other);

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "RuntimeDebugger.LogNode";
  }
  protected:
  explicit LogNode(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kLogTimeFieldNumber = 1,
    kLogMessageFieldNumber = 4,
    kLogStackTrackFieldNumber = 5,
    kLogFrameCountFieldNumber = 2,
    kLogTypeFieldNumber = 3,
  };
  // optional string log_time = 1;
  bool has_log_time() const;
  private:
  bool _internal_has_log_time() const;
  public:
  void clear_log_time();
  const std::string& log_time() const;
  template <typename ArgT0 = const std::string&, typename... ArgT>
  void set_log_time(ArgT0&& arg0, ArgT... args);
  std::string* mutable_log_time();
  PROTOBUF_NODISCARD std::string* release_log_time();
  void set_allocated_log_time(std::string* log_time);
  private:
  const std::string& _internal_log_time() const;
  inline PROTOBUF_ALWAYS_INLINE void _internal_set_log_time(const std::string& value);
  std::string* _internal_mutable_log_time();
  public:

  // optional string log_message = 4;
  bool has_log_message() const;
  private:
  bool _internal_has_log_message() const;
  public:
  void clear_log_message();
  const std::string& log_message() const;
  template <typename ArgT0 = const std::string&, typename... ArgT>
  void set_log_message(ArgT0&& arg0, ArgT... args);
  std::string* mutable_log_message();
  PROTOBUF_NODISCARD std::string* release_log_message();
  void set_allocated_log_message(std::string* log_message);
  private:
  const std::string& _internal_log_message() const;
  inline PROTOBUF_ALWAYS_INLINE void _internal_set_log_message(const std::string& value);
  std::string* _internal_mutable_log_message();
  public:

  // optional string log_stack_track = 5;
  bool has_log_stack_track() const;
  private:
  bool _internal_has_log_stack_track() const;
  public:
  void clear_log_stack_track();
  const std::string& log_stack_track() const;
  template <typename ArgT0 = const std::string&, typename... ArgT>
  void set_log_stack_track(ArgT0&& arg0, ArgT... args);
  std::string* mutable_log_stack_track();
  PROTOBUF_NODISCARD std::string* release_log_stack_track();
  void set_allocated_log_stack_track(std::string* log_stack_track);
  private:
  const std::string& _internal_log_stack_track() const;
  inline PROTOBUF_ALWAYS_INLINE void _internal_set_log_stack_track(const std::string& value);
  std::string* _internal_mutable_log_stack_track();
  public:

  // optional int32 log_frame_count = 2;
  bool has_log_frame_count() const;
  private:
  bool _internal_has_log_frame_count() const;
  public:
  void clear_log_frame_count();
  int32_t log_frame_count() const;
  void set_log_frame_count(int32_t value);
  private:
  int32_t _internal_log_frame_count() const;
  void _internal_set_log_frame_count(int32_t value);
  public:

  // optional .RuntimeDebugger.LogType log_type = 3;
  bool has_log_type() const;
  private:
  bool _internal_has_log_type() const;
  public:
  void clear_log_type();
  ::RuntimeDebugger::LogType log_type() const;
  void set_log_type(::RuntimeDebugger::LogType value);
  private:
  ::RuntimeDebugger::LogType _internal_log_type() const;
  void _internal_set_log_type(::RuntimeDebugger::LogType value);
  public:

  // @@protoc_insertion_point(class_scope:RuntimeDebugger.LogNode)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
    ::PROTOBUF_NAMESPACE_ID::internal::HasBits<1> _has_bits_;
    mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
    ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr log_time_;
    ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr log_message_;
    ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr log_stack_track_;
    int32_t log_frame_count_;
    int log_type_;
  };
  union { Impl_ _impl_; };
  friend struct ::TableStruct_runtime_5fdebugger_5flog_2eproto;
};
// -------------------------------------------------------------------

class LogList final :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:RuntimeDebugger.LogList) */ {
 public:
  inline LogList() : LogList(nullptr) {}
  ~LogList() override;
  explicit PROTOBUF_CONSTEXPR LogList(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  LogList(const LogList& from);
  LogList(LogList&& from) noexcept
    : LogList() {
    *this = ::std::move(from);
  }

  inline LogList& operator=(const LogList& from) {
    CopyFrom(from);
    return *this;
  }
  inline LogList& operator=(LogList&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const LogList& default_instance() {
    return *internal_default_instance();
  }
  static inline const LogList* internal_default_instance() {
    return reinterpret_cast<const LogList*>(
               &_LogList_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    1;

  friend void swap(LogList& a, LogList& b) {
    a.Swap(&b);
  }
  inline void Swap(LogList* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(LogList* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  LogList* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<LogList>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::Message::CopyFrom;
  void CopyFrom(const LogList& from);
  using ::PROTOBUF_NAMESPACE_ID::Message::MergeFrom;
  void MergeFrom( const LogList& from) {
    LogList::MergeImpl(*this, from);
  }
  private:
  static void MergeImpl(::PROTOBUF_NAMESPACE_ID::Message& to_msg, const ::PROTOBUF_NAMESPACE_ID::Message& from_msg);
  public:
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  uint8_t* _InternalSerialize(
      uint8_t* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _impl_._cached_size_.Get(); }

  private:
  void SharedCtor(::PROTOBUF_NAMESPACE_ID::Arena* arena, bool is_message_owned);
  void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(LogList* other);

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "RuntimeDebugger.LogList";
  }
  protected:
  explicit LogList(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kLogNodeListFieldNumber = 1,
  };
  // repeated .RuntimeDebugger.LogNode log_node_list = 1;
  int log_node_list_size() const;
  private:
  int _internal_log_node_list_size() const;
  public:
  void clear_log_node_list();
  ::RuntimeDebugger::LogNode* mutable_log_node_list(int index);
  ::PROTOBUF_NAMESPACE_ID::RepeatedPtrField< ::RuntimeDebugger::LogNode >*
      mutable_log_node_list();
  private:
  const ::RuntimeDebugger::LogNode& _internal_log_node_list(int index) const;
  ::RuntimeDebugger::LogNode* _internal_add_log_node_list();
  public:
  const ::RuntimeDebugger::LogNode& log_node_list(int index) const;
  ::RuntimeDebugger::LogNode* add_log_node_list();
  const ::PROTOBUF_NAMESPACE_ID::RepeatedPtrField< ::RuntimeDebugger::LogNode >&
      log_node_list() const;

  // @@protoc_insertion_point(class_scope:RuntimeDebugger.LogList)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
    ::PROTOBUF_NAMESPACE_ID::RepeatedPtrField< ::RuntimeDebugger::LogNode > log_node_list_;
    mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  };
  union { Impl_ _impl_; };
  friend struct ::TableStruct_runtime_5fdebugger_5flog_2eproto;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// LogNode

// optional string log_time = 1;
inline bool LogNode::_internal_has_log_time() const {
  bool value = (_impl_._has_bits_[0] & 0x00000001u) != 0;
  return value;
}
inline bool LogNode::has_log_time() const {
  return _internal_has_log_time();
}
inline void LogNode::clear_log_time() {
  _impl_.log_time_.ClearToEmpty();
  _impl_._has_bits_[0] &= ~0x00000001u;
}
inline const std::string& LogNode::log_time() const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogNode.log_time)
  return _internal_log_time();
}
template <typename ArgT0, typename... ArgT>
inline PROTOBUF_ALWAYS_INLINE
void LogNode::set_log_time(ArgT0&& arg0, ArgT... args) {
 _impl_._has_bits_[0] |= 0x00000001u;
 _impl_.log_time_.Set(static_cast<ArgT0 &&>(arg0), args..., GetArenaForAllocation());
  // @@protoc_insertion_point(field_set:RuntimeDebugger.LogNode.log_time)
}
inline std::string* LogNode::mutable_log_time() {
  std::string* _s = _internal_mutable_log_time();
  // @@protoc_insertion_point(field_mutable:RuntimeDebugger.LogNode.log_time)
  return _s;
}
inline const std::string& LogNode::_internal_log_time() const {
  return _impl_.log_time_.Get();
}
inline void LogNode::_internal_set_log_time(const std::string& value) {
  _impl_._has_bits_[0] |= 0x00000001u;
  _impl_.log_time_.Set(value, GetArenaForAllocation());
}
inline std::string* LogNode::_internal_mutable_log_time() {
  _impl_._has_bits_[0] |= 0x00000001u;
  return _impl_.log_time_.Mutable(GetArenaForAllocation());
}
inline std::string* LogNode::release_log_time() {
  // @@protoc_insertion_point(field_release:RuntimeDebugger.LogNode.log_time)
  if (!_internal_has_log_time()) {
    return nullptr;
  }
  _impl_._has_bits_[0] &= ~0x00000001u;
  auto* p = _impl_.log_time_.Release();
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_time_.IsDefault()) {
    _impl_.log_time_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  return p;
}
inline void LogNode::set_allocated_log_time(std::string* log_time) {
  if (log_time != nullptr) {
    _impl_._has_bits_[0] |= 0x00000001u;
  } else {
    _impl_._has_bits_[0] &= ~0x00000001u;
  }
  _impl_.log_time_.SetAllocated(log_time, GetArenaForAllocation());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_time_.IsDefault()) {
    _impl_.log_time_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  // @@protoc_insertion_point(field_set_allocated:RuntimeDebugger.LogNode.log_time)
}

// optional int32 log_frame_count = 2;
inline bool LogNode::_internal_has_log_frame_count() const {
  bool value = (_impl_._has_bits_[0] & 0x00000008u) != 0;
  return value;
}
inline bool LogNode::has_log_frame_count() const {
  return _internal_has_log_frame_count();
}
inline void LogNode::clear_log_frame_count() {
  _impl_.log_frame_count_ = 0;
  _impl_._has_bits_[0] &= ~0x00000008u;
}
inline int32_t LogNode::_internal_log_frame_count() const {
  return _impl_.log_frame_count_;
}
inline int32_t LogNode::log_frame_count() const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogNode.log_frame_count)
  return _internal_log_frame_count();
}
inline void LogNode::_internal_set_log_frame_count(int32_t value) {
  _impl_._has_bits_[0] |= 0x00000008u;
  _impl_.log_frame_count_ = value;
}
inline void LogNode::set_log_frame_count(int32_t value) {
  _internal_set_log_frame_count(value);
  // @@protoc_insertion_point(field_set:RuntimeDebugger.LogNode.log_frame_count)
}

// optional .RuntimeDebugger.LogType log_type = 3;
inline bool LogNode::_internal_has_log_type() const {
  bool value = (_impl_._has_bits_[0] & 0x00000010u) != 0;
  return value;
}
inline bool LogNode::has_log_type() const {
  return _internal_has_log_type();
}
inline void LogNode::clear_log_type() {
  _impl_.log_type_ = 0;
  _impl_._has_bits_[0] &= ~0x00000010u;
}
inline ::RuntimeDebugger::LogType LogNode::_internal_log_type() const {
  return static_cast< ::RuntimeDebugger::LogType >(_impl_.log_type_);
}
inline ::RuntimeDebugger::LogType LogNode::log_type() const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogNode.log_type)
  return _internal_log_type();
}
inline void LogNode::_internal_set_log_type(::RuntimeDebugger::LogType value) {
  _impl_._has_bits_[0] |= 0x00000010u;
  _impl_.log_type_ = value;
}
inline void LogNode::set_log_type(::RuntimeDebugger::LogType value) {
  _internal_set_log_type(value);
  // @@protoc_insertion_point(field_set:RuntimeDebugger.LogNode.log_type)
}

// optional string log_message = 4;
inline bool LogNode::_internal_has_log_message() const {
  bool value = (_impl_._has_bits_[0] & 0x00000002u) != 0;
  return value;
}
inline bool LogNode::has_log_message() const {
  return _internal_has_log_message();
}
inline void LogNode::clear_log_message() {
  _impl_.log_message_.ClearToEmpty();
  _impl_._has_bits_[0] &= ~0x00000002u;
}
inline const std::string& LogNode::log_message() const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogNode.log_message)
  return _internal_log_message();
}
template <typename ArgT0, typename... ArgT>
inline PROTOBUF_ALWAYS_INLINE
void LogNode::set_log_message(ArgT0&& arg0, ArgT... args) {
 _impl_._has_bits_[0] |= 0x00000002u;
 _impl_.log_message_.Set(static_cast<ArgT0 &&>(arg0), args..., GetArenaForAllocation());
  // @@protoc_insertion_point(field_set:RuntimeDebugger.LogNode.log_message)
}
inline std::string* LogNode::mutable_log_message() {
  std::string* _s = _internal_mutable_log_message();
  // @@protoc_insertion_point(field_mutable:RuntimeDebugger.LogNode.log_message)
  return _s;
}
inline const std::string& LogNode::_internal_log_message() const {
  return _impl_.log_message_.Get();
}
inline void LogNode::_internal_set_log_message(const std::string& value) {
  _impl_._has_bits_[0] |= 0x00000002u;
  _impl_.log_message_.Set(value, GetArenaForAllocation());
}
inline std::string* LogNode::_internal_mutable_log_message() {
  _impl_._has_bits_[0] |= 0x00000002u;
  return _impl_.log_message_.Mutable(GetArenaForAllocation());
}
inline std::string* LogNode::release_log_message() {
  // @@protoc_insertion_point(field_release:RuntimeDebugger.LogNode.log_message)
  if (!_internal_has_log_message()) {
    return nullptr;
  }
  _impl_._has_bits_[0] &= ~0x00000002u;
  auto* p = _impl_.log_message_.Release();
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_message_.IsDefault()) {
    _impl_.log_message_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  return p;
}
inline void LogNode::set_allocated_log_message(std::string* log_message) {
  if (log_message != nullptr) {
    _impl_._has_bits_[0] |= 0x00000002u;
  } else {
    _impl_._has_bits_[0] &= ~0x00000002u;
  }
  _impl_.log_message_.SetAllocated(log_message, GetArenaForAllocation());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_message_.IsDefault()) {
    _impl_.log_message_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  // @@protoc_insertion_point(field_set_allocated:RuntimeDebugger.LogNode.log_message)
}

// optional string log_stack_track = 5;
inline bool LogNode::_internal_has_log_stack_track() const {
  bool value = (_impl_._has_bits_[0] & 0x00000004u) != 0;
  return value;
}
inline bool LogNode::has_log_stack_track() const {
  return _internal_has_log_stack_track();
}
inline void LogNode::clear_log_stack_track() {
  _impl_.log_stack_track_.ClearToEmpty();
  _impl_._has_bits_[0] &= ~0x00000004u;
}
inline const std::string& LogNode::log_stack_track() const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogNode.log_stack_track)
  return _internal_log_stack_track();
}
template <typename ArgT0, typename... ArgT>
inline PROTOBUF_ALWAYS_INLINE
void LogNode::set_log_stack_track(ArgT0&& arg0, ArgT... args) {
 _impl_._has_bits_[0] |= 0x00000004u;
 _impl_.log_stack_track_.Set(static_cast<ArgT0 &&>(arg0), args..., GetArenaForAllocation());
  // @@protoc_insertion_point(field_set:RuntimeDebugger.LogNode.log_stack_track)
}
inline std::string* LogNode::mutable_log_stack_track() {
  std::string* _s = _internal_mutable_log_stack_track();
  // @@protoc_insertion_point(field_mutable:RuntimeDebugger.LogNode.log_stack_track)
  return _s;
}
inline const std::string& LogNode::_internal_log_stack_track() const {
  return _impl_.log_stack_track_.Get();
}
inline void LogNode::_internal_set_log_stack_track(const std::string& value) {
  _impl_._has_bits_[0] |= 0x00000004u;
  _impl_.log_stack_track_.Set(value, GetArenaForAllocation());
}
inline std::string* LogNode::_internal_mutable_log_stack_track() {
  _impl_._has_bits_[0] |= 0x00000004u;
  return _impl_.log_stack_track_.Mutable(GetArenaForAllocation());
}
inline std::string* LogNode::release_log_stack_track() {
  // @@protoc_insertion_point(field_release:RuntimeDebugger.LogNode.log_stack_track)
  if (!_internal_has_log_stack_track()) {
    return nullptr;
  }
  _impl_._has_bits_[0] &= ~0x00000004u;
  auto* p = _impl_.log_stack_track_.Release();
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_stack_track_.IsDefault()) {
    _impl_.log_stack_track_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  return p;
}
inline void LogNode::set_allocated_log_stack_track(std::string* log_stack_track) {
  if (log_stack_track != nullptr) {
    _impl_._has_bits_[0] |= 0x00000004u;
  } else {
    _impl_._has_bits_[0] &= ~0x00000004u;
  }
  _impl_.log_stack_track_.SetAllocated(log_stack_track, GetArenaForAllocation());
#ifdef PROTOBUF_FORCE_COPY_DEFAULT_STRING
  if (_impl_.log_stack_track_.IsDefault()) {
    _impl_.log_stack_track_.Set("", GetArenaForAllocation());
  }
#endif // PROTOBUF_FORCE_COPY_DEFAULT_STRING
  // @@protoc_insertion_point(field_set_allocated:RuntimeDebugger.LogNode.log_stack_track)
}

// -------------------------------------------------------------------

// LogList

// repeated .RuntimeDebugger.LogNode log_node_list = 1;
inline int LogList::_internal_log_node_list_size() const {
  return _impl_.log_node_list_.size();
}
inline int LogList::log_node_list_size() const {
  return _internal_log_node_list_size();
}
inline void LogList::clear_log_node_list() {
  _impl_.log_node_list_.Clear();
}
inline ::RuntimeDebugger::LogNode* LogList::mutable_log_node_list(int index) {
  // @@protoc_insertion_point(field_mutable:RuntimeDebugger.LogList.log_node_list)
  return _impl_.log_node_list_.Mutable(index);
}
inline ::PROTOBUF_NAMESPACE_ID::RepeatedPtrField< ::RuntimeDebugger::LogNode >*
LogList::mutable_log_node_list() {
  // @@protoc_insertion_point(field_mutable_list:RuntimeDebugger.LogList.log_node_list)
  return &_impl_.log_node_list_;
}
inline const ::RuntimeDebugger::LogNode& LogList::_internal_log_node_list(int index) const {
  return _impl_.log_node_list_.Get(index);
}
inline const ::RuntimeDebugger::LogNode& LogList::log_node_list(int index) const {
  // @@protoc_insertion_point(field_get:RuntimeDebugger.LogList.log_node_list)
  return _internal_log_node_list(index);
}
inline ::RuntimeDebugger::LogNode* LogList::_internal_add_log_node_list() {
  return _impl_.log_node_list_.Add();
}
inline ::RuntimeDebugger::LogNode* LogList::add_log_node_list() {
  ::RuntimeDebugger::LogNode* _add = _internal_add_log_node_list();
  // @@protoc_insertion_point(field_add:RuntimeDebugger.LogList.log_node_list)
  return _add;
}
inline const ::PROTOBUF_NAMESPACE_ID::RepeatedPtrField< ::RuntimeDebugger::LogNode >&
LogList::log_node_list() const {
  // @@protoc_insertion_point(field_list:RuntimeDebugger.LogList.log_node_list)
  return _impl_.log_node_list_;
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__
// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)

}  // namespace RuntimeDebugger

PROTOBUF_NAMESPACE_OPEN

template <> struct is_proto_enum< ::RuntimeDebugger::LogType> : ::std::true_type {};
template <>
inline const EnumDescriptor* GetEnumDescriptor< ::RuntimeDebugger::LogType>() {
  return ::RuntimeDebugger::LogType_descriptor();
}

PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_runtime_5fdebugger_5flog_2eproto
